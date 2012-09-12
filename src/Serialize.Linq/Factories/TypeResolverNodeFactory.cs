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
        
        public TypeResolverNodeFactory(IEnumerable<Type> expectedTypes)
        {
            if(expectedTypes == null)
                throw new ArgumentNullException("expectedTypes");
            _expectedTypes = expectedTypes.ToArray();            
        }

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

        private bool TryGetConstantValueFromMemberExpression(MemberExpression memberExpression, out object constantValue)
        {
            constantValue = null;

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
                    if (memberExpression.Expression.NodeType == ExpressionType.Constant)
                    {
                        var constantExpression = (ConstantExpression) memberExpression.Expression;
                        var fields = constantExpression.Type.GetFields();
                        var memberField = fields.Single(n => memberExpression.Member.Name.Contains(n.Name));
                        constantValue = memberField.GetValue(constantExpression.Value);
                        return true;
                    }
                    memberExpression = (MemberExpression) memberExpression.Expression;
                    return this.TryGetConstantValueFromMemberExpression(memberExpression, out constantValue);

                case MemberTypes.Property:
                    constantValue = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                    return true;

                default:
                    throw new NotSupportedException("MemberType '" + memberExpression.Member.MemberType + "' not yet supported.");
            }
        }

        private ExpressionNode ResolveMemberExpression(MemberExpression memberExpression)
        {
            object constantValue;
            return this.TryGetConstantValueFromMemberExpression(memberExpression, out constantValue) 
                ? new ConstantExpressionNode(constantValue) 
                : base.Create(memberExpression);
        }

        private ExpressionNode ResolveMethodCallExpression(MethodCallExpression methodCallExpression)
        {
            var memberExpression = methodCallExpression.Object as MemberExpression;
            if (memberExpression != null)
            {
                object constantValue;
                if(this.TryGetConstantValueFromMemberExpression(memberExpression, out constantValue))
                    return new ConstantExpressionNode(Expression.Lambda(methodCallExpression).Compile().DynamicInvoke());
            }
            else if (methodCallExpression.Method.Name == "ToString" && methodCallExpression.Method.ReturnType == typeof(string))
		    {
			    var constantValue = Expression.Lambda(methodCallExpression).Compile().DynamicInvoke();
				return new ConstantExpressionNode(constantValue);				
            }
            return base.Create(methodCallExpression);
        }

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