using System;
using System.Linq.Expressions;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public class ExpressionNodeFactory : IExpressionNodeFactory
    {
        public virtual ExpressionNode Create(Expression expression)
        {
            if (expression == null)
                return null;

            if (expression is BinaryExpression)        return new BinaryExpressionNode(this, expression as BinaryExpression);
            if (expression is ConditionalExpression)   return new ConditionalExpressionNode(this, expression as ConditionalExpression);
            if (expression is ConstantExpression)      return new ConstantExpressionNode(this, expression as ConstantExpression);
            if (expression is InvocationExpression)    return new InvocationExpressionNode(this, expression as InvocationExpression);
            if (expression is LambdaExpression)        return new LambdaExpressionNode(this, expression as LambdaExpression);
            if (expression is ListInitExpression)      return new ListInitExpressionNode(this, expression as ListInitExpression);
            if (expression is MemberExpression)        return new MemberExpressionNode(this, expression as MemberExpression);
            if (expression is MemberInitExpression)    return new MemberInitExpressionNode(this, expression as MemberInitExpression);
            if (expression is MethodCallExpression)    return new MethodCallExpressionNode(this, expression as MethodCallExpression);
            if (expression is NewArrayExpression)      return new NewArrayExpressionNode(this, expression as NewArrayExpression);
            if (expression is NewExpression)           return new NewExpressionNode(this, expression as NewExpression);
            if (expression is ParameterExpression)     return new ParameterExpressionNode(this, expression as ParameterExpression);                        
            if (expression is TypeBinaryExpression)    return new TypeBinaryExpressionNode(this, expression as TypeBinaryExpression);
            if (expression is UnaryExpression)         return new UnaryExpressionNode(this, expression as UnaryExpression);                        

            throw new ArgumentException("Unknown expression of type " + expression.GetType());
        }
    }
}