using System;
using System.Linq.Expressions;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public class NodeFactory : INodeFactory
    {                
        public virtual ExpressionNode Create(Expression Expression)
        {
            if (Expression == null)
                return null;

            if (Expression is BinaryExpression)        return new BinaryExpressionNode(this, Expression as BinaryExpression);
            if (Expression is ConditionalExpression)   return new ConditionalExpressionNode(this, Expression as ConditionalExpression);
            if (Expression is ConstantExpression)      return new ConstantExpressionNode(this, Expression as ConstantExpression);
            if (Expression is InvocationExpression)    return new InvocationExpressionNode(this, Expression as InvocationExpression);
            if (Expression is LambdaExpression)        return new LambdaExpressionNode(this, Expression as LambdaExpression);
            if (Expression is ListInitExpression)      return new ListInitExpressionNode(this, Expression as ListInitExpression);
            if (Expression is MemberExpression)        return new MemberExpressionNode(this, Expression as MemberExpression);
            if (Expression is MemberInitExpression)    return new MemberInitExpressionNode(this, Expression as MemberInitExpression);
            if (Expression is MethodCallExpression)    return new MethodCallExpressionNode(this, Expression as MethodCallExpression);
            if (Expression is NewArrayExpression)      return new NewArrayExpressionNode(this, Expression as NewArrayExpression);
            if (Expression is NewExpression)           return new NewExpressionNode(this, Expression as NewExpression);
            if (Expression is ParameterExpression)     return new ParameterExpressionNode(this, Expression as ParameterExpression);                        
            if (Expression is TypeBinaryExpression)    return new TypeBinaryExpressionNode(this, Expression as TypeBinaryExpression);
            if (Expression is UnaryExpression)         return new UnaryExpressionNode(this, Expression as UnaryExpression);                        

            throw new ArgumentException("Unknown Expression of type " + Expression.GetType());
        }

        public TypeNode Create(Type type)
        {
            return new TypeNode(this, type);
        }
    }
}