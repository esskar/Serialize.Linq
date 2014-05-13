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
        /// <exception cref="System.ArgumentNullException">expectedTypes</exception>
        public TypeResolverNodeFactory(IEnumerable<Type> expectedTypes)
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
                if (declaredType == expectedType || declaredType.IsSubclassOf(expectedType))
                    return true;
                if (expectedType.IsInterface)
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
            while (run != null)
            {
                if (this.IsExpectedType(run.Member.DeclaringType))
                    return false;

                run = run.Expression as MemberExpression;
            }

            switch (memberExpression.Member.MemberType)
            {
                case MemberTypes.Field:
                    if (memberExpression.Expression != null)
                    {
                        if (memberExpression.Expression.NodeType == ExpressionType.Constant)
                        {
                            var constantExpression = (ConstantExpression)memberExpression.Expression;
                            var fields = constantExpression.Type.GetFields();
                            var memberField = fields.Single(n => memberExpression.Member.Name.Contains(n.Name));
                            constantValueType = memberField.FieldType;
                            constantValue = memberField.GetValue(constantExpression.Value);
                            return true;
                        }
                        memberExpression = (MemberExpression)memberExpression.Expression;
                        return this.TryGetConstantValueFromMemberExpression(memberExpression, out constantValue, out constantValueType);
                    }
                    var field = (FieldInfo)memberExpression.Member;
                    if (field.IsPrivate || field.IsFamilyAndAssembly)
                    {
                        constantValue = field.GetValue(null);
                        return true;
                    }
                    break;

                case MemberTypes.Property:
                    constantValue = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                    return true;

                default:
                    throw new NotSupportedException("MemberType '" + memberExpression.Member.MemberType + "' not yet supported.");
            }

            return false;
        }

        /// <summary>
        /// Resolves the member expression.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        /// <returns></returns>
        private ExpressionNode ResolveMemberExpression(MemberExpression memberExpression)
        {
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
            if (expression is MemberExpression)
                return this.ResolveMemberExpression(expression as MemberExpression);
            if (expression is MethodCallExpression)
                return this.ResolveMethodCallExpression(expression as MethodCallExpression);
            return base.Create(expression);
        }
    }
}