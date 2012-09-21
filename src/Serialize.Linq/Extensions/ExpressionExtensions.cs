using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Extensions
{
    public static class ExpressionExtensions
    {
        public static ExpressionNode ToExpressionNode(this Expression Expression)
        {
            var converter = new ExpressionConverter();
            return converter.Convert(Expression);
        }

        public static string ToJson(this Expression Expression)
        {
            return Expression.ToJson(Expression.GetDefaultFactory());
        }

        public static string ToJson(this Expression Expression, INodeFactory factory)
        {
            return Expression.ToJson(factory, new JsonSerializer());
        }

        public static string ToJson(this Expression Expression, INodeFactory factory, IJsonSerializer serializer)
        {
            return Expression.ToText(factory, serializer);
        }

        public static string ToXml(this Expression Expression)
        {
            return Expression.ToXml(Expression.GetDefaultFactory());
        }

        public static string ToXml(this Expression Expression, INodeFactory factory)
        {
            return Expression.ToXml(factory, new XmlSerializer());
        }

        public static string ToXml(this Expression Expression, INodeFactory factory, IXmlSerializer serializer)
        {
            return Expression.ToText(factory, serializer);
        }

        public static string ToText(this Expression Expression, INodeFactory factory, ITextSerializer serializer)
        {            
            if(factory == null)
                throw new ArgumentNullException("factory");
            if(serializer == null)
                throw new ArgumentNullException("serializer");

            var ExpressionNode = factory.Create(Expression);
            return serializer.Serialize(ExpressionNode);
        }

        internal static INodeFactory GetDefaultFactory(this Expression Expression)
        {
            var lambda = Expression as LambdaExpression;
            if(lambda != null)
                return  new DefaultNodeFactory(lambda.Parameters.Select(p => p.Type));
            return new NodeFactory();
        }

        internal static IEnumerable<Expression> GetLinkNodes(this Expression Expression)
        {
            if (Expression is LambdaExpression)
            {
                var lambdaExpression = (LambdaExpression)Expression;

                yield return lambdaExpression.Body;
                foreach (var parameter in lambdaExpression.Parameters)
                    yield return parameter;
            }
            else if (Expression is BinaryExpression)
            {
                var binaryExpression = (BinaryExpression)Expression;

                yield return binaryExpression.Left;
                yield return binaryExpression.Right;
            }
            else if (Expression is ConditionalExpression)
            {
                var conditionalExpression = (ConditionalExpression)Expression;

                yield return conditionalExpression.IfTrue;
                yield return conditionalExpression.IfFalse;
                yield return conditionalExpression.Test;
            }
            else if (Expression is InvocationExpression)
            {
                var invocationExpression = (InvocationExpression)Expression;
                yield return invocationExpression.Expression;
                foreach (var argument in invocationExpression.Arguments)
                    yield return argument;                
            }
            else if (Expression is ListInitExpression)
            {
                yield return (Expression as ListInitExpression).NewExpression;
            }
            else if (Expression is MemberExpression)
            {
                yield return (Expression as MemberExpression).Expression;
            }
            else if (Expression is MemberInitExpression)
            {
                yield return (Expression as MemberInitExpression).NewExpression;
            }
            else if (Expression is MethodCallExpression)
            {
                var methodCallExpression = (MethodCallExpression)Expression;
                foreach (var argument in methodCallExpression.Arguments)
                    yield return argument;
                if (methodCallExpression.Object != null)                
                    yield return methodCallExpression.Object;                
            }
            else if (Expression is NewArrayExpression)
            {
                foreach (var item in (Expression as NewArrayExpression).Expressions)
                    yield return item;
            }
            else if (Expression is NewExpression)
            {
                foreach (var item in (Expression as NewExpression).Arguments)
                    yield return item;
            }
            else if (Expression is TypeBinaryExpression)
            {
                yield return (Expression as TypeBinaryExpression).Expression;
            }
            else if (Expression is UnaryExpression)
            {
                yield return (Expression as UnaryExpression).Operand;
            }
        }

        internal static IEnumerable<Expression> GetNodes(this Expression Expression)
        {
            foreach (var node in Expression.GetLinkNodes())
            {
                foreach (var subNode in node.GetNodes())
                    yield return subNode;
            }
            yield return Expression;
        }

        internal static IEnumerable<TExpression> GetNodes<TExpression>(this Expression Expression) where TExpression : Expression
        {
            return Expression.GetNodes().OfType<TExpression>();
        }
    }
}