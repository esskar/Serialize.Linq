
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Serialize.Linq.Internals;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public class TypeResolverNodeFactory : NodeFactory
    {
        private readonly Type[] _expectedTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeResolverNodeFactory"/> class.
        /// </summary>
        /// <param name="expectedTypes">The expected types.</param>
        /// <param name="factorySettings">The factory settings to use.</param>
        /// <exception cref="System.ArgumentNullException">expectedTypes</exception>
        public TypeResolverNodeFactory(IEnumerable<Type> expectedTypes, FactorySettings factorySettings = null)
            : base(factorySettings)
        {
            if (expectedTypes == null)
                throw new ArgumentNullException(nameof(expectedTypes));
            _expectedTypes = expectedTypes.ToArray();
        }

        /// <summary>
        /// Determines whether the specified type is expected.
        /// </summary>
        /// <param name="declaredType">Type of the declared.</param>
        /// <returns>
        ///   <c>true</c> if type is expected; otherwise, <c>false</c>.
        /// </returns>
        private bool IsExpectedType(Type declaredType)
        {
            foreach (var expectedType in _expectedTypes)
            {
                if (declaredType == expectedType || declaredType.IsSubclassOf(expectedType))
                    return true;
                if (expectedType.IsInterface())
                {
                    var resultTypes = declaredType.GetInterfaces();
                    if (resultTypes.Contains(expectedType))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries the get constant value from member expression.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        /// <param name="constantValue">The constant value.</param>
        /// <param name="constantValueType">Type of the constant value.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">MemberType ' + memberExpression.Member.MemberType + ' not yet supported.</exception>
        private bool TryGetConstantValueFromMemberExpression(
            MemberExpression memberExpression,
            out object constantValue,
            out Type constantValueType)
        {
            constantValue = null;
            constantValueType = null;

            FieldInfo parentField = null;
            while (true)
            {
                var run = memberExpression;
                while (true)
                {
                    if (!(run.Expression is MemberExpression next))
                        break;
                    run = next;
                }

                if (IsExpectedType(run.Member.DeclaringType))
                    return false;

                switch (memberExpression.Member)
                {
                    case FieldInfo field:
                        {
                            if (memberExpression.Expression != null)
                            {
                                if (memberExpression.Expression.NodeType == ExpressionType.Constant)
                                {
                                    var constantExpression = (ConstantExpression)memberExpression.Expression;
                                    var flags = GetBindingFlags();

                                    constantValue = constantExpression.Value;
                                    constantValueType = constantExpression.Type;
                                    var match = false;
                                    do
                                    {
                                        var fields = flags == null
                                            ? constantValueType.GetFields()
                                            : constantValueType.GetFields(flags.Value);
                                        var memberField = fields.Length > 1
                                            ? fields.SingleOrDefault(n => field.Name.Equals(n.Name))
                                            : fields.FirstOrDefault();
                                        if (memberField == null && parentField != null)
                                        {
                                            memberField = fields.Length > 1
                                                ? fields.SingleOrDefault(n => parentField.Name.Equals(n.Name))
                                                : fields.FirstOrDefault();
                                        }
                                        if (memberField == null)
                                            break;

                                        constantValueType = memberField.FieldType;
                                        constantValue = memberField.GetValue(constantValue);
                                        match = true;
                                    }
                                    while (constantValue != null && !KnownTypes.Match(constantValueType));

                                    return match;
                                }

                                if (memberExpression.Expression is MemberExpression subExpression)
                                {
                                    memberExpression = subExpression;
                                    parentField = parentField ?? field;
                                    continue;
                                }
                            }

                            if (field.IsPrivate || field.IsFamilyAndAssembly)
                            {
                                constantValue = field.GetValue(null);
                                return true;
                            }

                            break;
                        }
                    case PropertyInfo propertyInfo:
                        try
                        {
                            constantValue = Expression.Lambda(memberExpression).Compile().DynamicInvoke();

                            constantValueType = propertyInfo.PropertyType;
                            return true;
                        }
                        catch (InvalidOperationException)
                        {
                            constantValue = null;
                            return false;
                        }
                    default:
                        throw new NotSupportedException("MemberType '" + memberExpression.Member.GetType().Name + "' not yet supported.");
                }

                return false;
            }
        }

        /// <summary>
        /// Tries to inline an expression.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        /// <param name="inlineExpression">The inline expression.</param>
        /// <returns></returns>
        private bool TryToInlineExpression(MemberExpression memberExpression, out Expression inlineExpression)
        {           
            inlineExpression = null;
            
            if (!(memberExpression.Member is FieldInfo) && !(memberExpression.Member is PropertyInfo))
            {
                return false;
            }

            if (memberExpression.Expression == null || memberExpression.Expression.NodeType != ExpressionType.Constant)
            {
                return false;
            }

            var constantExpression = (ConstantExpression)memberExpression.Expression;
            var flags = GetBindingFlags();


            object constantValue = null;

            if (memberExpression.Member is FieldInfo)
            {
                var fields = flags == null ? constantExpression.Type.GetFields() : constantExpression.Type.GetFields(flags.Value);
                var memberField = fields.Single(n => memberExpression.Member.Name.Equals(n.Name));

                constantValue = memberField.GetValue(constantExpression.Value);
            }
            else if (memberExpression.Member is PropertyInfo)
            {
                var properties = flags == null ? constantExpression.Type.GetProperties() : constantExpression.Type.GetProperties(flags.Value);
                var memberProperty = properties.Single(n => memberExpression.Member.Name.Equals(n.Name));

                constantValue = memberProperty.GetValue(constantExpression.Value, null);
            }

            inlineExpression = constantValue as Expression;

            return inlineExpression != null;
        }

        /// <summary>
        /// Resolves the member expression.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        /// <returns></returns>
        private ExpressionNode ResolveMemberExpression(MemberExpression memberExpression)
        {
            if (TryToInlineExpression(memberExpression, out var inlineExpression))
                return Create(inlineExpression);

            return TryGetConstantValueFromMemberExpression(memberExpression, out var constantValue, out var constantValueType)
                ? new ConstantExpressionNode(this, constantValue, constantValueType)
                : base.Create(memberExpression);
        }

        /// <summary>
        /// Resolves the method call expression.
        /// </summary>
        /// <param name="methodCallExpression">The method call expression.</param>
        /// <returns></returns>
        private ExpressionNode ResolveMethodCallExpression(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Object is MemberExpression memberExpression)
            {
                if (TryGetConstantValueFromMemberExpression(memberExpression, out _, out _))
                {
                    if (methodCallExpression.Arguments.Count == 0)
                        return new ConstantExpressionNode(this, Expression.Lambda(methodCallExpression).Compile().DynamicInvoke());
                }
            }
            else if (methodCallExpression.Method.Name == "ToString" && methodCallExpression.Method.ReturnType == typeof(string))
            {
                var constantValue = Expression.Lambda(methodCallExpression).Compile().DynamicInvoke();
                return new ConstantExpressionNode(this, constantValue);
            }
            return base.Create(methodCallExpression);
        }

        /// <summary>
        /// Creates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public override ExpressionNode Create(Expression expression)
        {
            if (expression is MemberExpression member)
                return ResolveMemberExpression(member);

            if (expression is MethodCallExpression method)
                return ResolveMethodCallExpression(method);

            return base.Create(expression);
        }
    }
}