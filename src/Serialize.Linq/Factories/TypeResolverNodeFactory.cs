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
                throw new ArgumentNullException("expectedTypes");
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
                if (declaredType == expectedType || declaredType.GetTypeInfo().IsSubclassOf(expectedType))
                    return true;
                if (expectedType.GetTypeInfo().IsInterface)
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
        private bool TryGetConstantValueFromMemberExpression(MemberExpression memberExpression, out object constantValue, out Type constantValueType)
        {
            constantValue = null;
            constantValueType = null;

            var run = memberExpression;
            while (true)
            {
                var next = run.Expression as MemberExpression;
                if (next == null)
                    break;

                run = next;
            }

            if (this.IsExpectedType(run.Member.DeclaringType))
                return false;

            var field = memberExpression.Member as FieldInfo;
            if (field != null)
            {
                if (memberExpression.Expression != null)
                {
                    if (memberExpression.Expression.NodeType == ExpressionType.Constant)
                    {
                        var constantExpression = (ConstantExpression)memberExpression.Expression;
                        var flags = this.GetBindingFlags();
                        var fields = flags == null ? constantExpression.Type.GetFields() : constantExpression.Type.GetFields(flags.Value);
                        var memberField = fields.Single(n => memberExpression.Member.Name.Equals(n.Name));
                        constantValueType = memberField.FieldType;
                        constantValue = memberField.GetValue(constantExpression.Value);
                        return true;
                    }
                    var subExpression = memberExpression.Expression as MemberExpression;
                    if (subExpression != null)
                        return this.TryGetConstantValueFromMemberExpression(subExpression, out constantValue, out constantValueType);
                }
                if (field.IsPrivate || field.IsFamilyAndAssembly)
                {
                    constantValue = field.GetValue(null);
                    return true;
                }
            }
            else if (memberExpression.Member is PropertyInfo)
            {
                try
                {
                    constantValue = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                    return true;
                }
                catch (InvalidOperationException)
                {
                    constantValue = null;
                    return false;
                }
            }
            else
            {
                throw new NotSupportedException("MemberType '" + memberExpression.Member.GetType().Name + "' not yet supported.");
            }

            return false;
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


            if (!(memberExpression.Member is FieldInfo))
                return false;

            if (memberExpression.Expression == null || memberExpression.Expression.NodeType != ExpressionType.Constant)
                return false;

            var constantExpression = (ConstantExpression)memberExpression.Expression;
            var flags = this.GetBindingFlags();
            var fields = flags == null
                ? constantExpression.Type.GetFields()
                : constantExpression.Type.GetFields(flags.Value);
            var memberField = fields.Single(n => memberExpression.Member.Name.Equals(n.Name));
            var constantValue = memberField.GetValue(constantExpression.Value);

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
            Expression inlineExpression;
            if (this.TryToInlineExpression(memberExpression, out inlineExpression))
                return this.Create(inlineExpression);

            object constantValue;
            Type constantValueType;

            return this.TryGetConstantValueFromMemberExpression(memberExpression, out constantValue, out constantValueType)
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
            var memberExpression = methodCallExpression.Object as MemberExpression;
            if (memberExpression != null)
            {
                object constantValue;
                Type constantValueType;
                if (this.TryGetConstantValueFromMemberExpression(memberExpression, out constantValue, out constantValueType))
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
            var member = expression as MemberExpression;
            if (member != null)
                return this.ResolveMemberExpression(member);

            var method = expression as MethodCallExpression;
            if (method != null)
                return this.ResolveMethodCallExpression(method);

            return base.Create(expression);
        }
    }
}