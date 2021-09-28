#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

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
        private readonly IEnumerable<Type> _expectedTypes;

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
            if (expectedTypes.Any(t => t == null))
                throw new ArgumentException("All types must be non-null.", nameof(expectedTypes));
            _expectedTypes = expectedTypes;
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
            return _expectedTypes.Any(type => declaredType == type ||
                                      declaredType.IsSubclassOf(type) ||
                                      (type.IsInterface() && declaredType.GetInterfaces().Contains(type)));
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

                if (this.IsExpectedType(run.Member.DeclaringType))
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

                                    constantValue = constantExpression.Value;
                                    constantValueType = constantExpression.Type;
                                    var match = false;
                                    do
                                    {
                                        var fields = constantValueType.GetFields(this.Settings.BindingFlags);
                                        FieldInfo memberField = null;
                                        if (fields.Length > 0)
                                        {
                                            if (fields.Length == 1)
                                            {
                                                memberField = fields[0];
                                            }
                                            else
                                            {
                                                memberField = fields.SingleOrDefault(n => field.Name.Equals(n.Name));
                                            }
                                            if (memberField == null && parentField != null && fields.Length > 1)
                                            {
                                                memberField = fields.SingleOrDefault(n => parentField.Name.Equals(n.Name));
                                            }
                                        }
                                        if (memberField == null)
                                            break;

                                        constantValueType = memberField.FieldType;
                                        constantValue = memberField.GetValue(constantValue);
                                        match = true;
                                    }
                                    while (constantValue != null && !(KnownTypes.Match(constantValueType) || KnownTypes.TryAddAsAssignable(constantValueType)));

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
                                if (!KnownTypes.Match(constantValueType))
                                {
                                    KnownTypes.TryAddAsAssignable(constantValueType);
                                }
                                return true;
                            }

                            break;
                        }
                    case PropertyInfo propertyInfo:
                        try
                        {
                            constantValue = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                            constantValueType = propertyInfo.PropertyType;
                            if (!KnownTypes.Match(constantValueType))
                            {
                                KnownTypes.TryAddAsAssignable(constantValueType);
                            }
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

            if (!(memberExpression.Member is FieldInfo) && !(memberExpression.Member is System.Reflection.PropertyInfo))
            {
                return false;
            }

            if (memberExpression.Expression == null || memberExpression.Expression.NodeType != ExpressionType.Constant)
            {
                return false;
            }

            var constantExpression = (ConstantExpression)memberExpression.Expression;

            object constantValue = null;

            if (memberExpression.Member is FieldInfo)
            {
                var fields = constantExpression.Type.GetFields(this.Settings.BindingFlags);
                var memberField = fields.Single(n => memberExpression.Member.Name.Equals(n.Name));

                constantValue = memberField.GetValue(constantExpression.Value);
            }
            else if (memberExpression.Member is PropertyInfo)
            {
                var properties = constantExpression.Type.GetProperties(this.Settings.BindingFlags);
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
            if (this.TryToInlineExpression(memberExpression, out var inlineExpression))
                return this.Create(inlineExpression);

            return this.TryGetConstantValueFromMemberExpression(memberExpression, out var constantValue, out var constantValueType)
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
                if (this.TryGetConstantValueFromMemberExpression(memberExpression, out _, out _))
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
                return this.ResolveMemberExpression(member);

            if (expression is MethodCallExpression method)
                return this.ResolveMethodCallExpression(method);

            return base.Create(expression);
        }
    }
}