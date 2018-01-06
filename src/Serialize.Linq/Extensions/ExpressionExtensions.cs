using System;
using System.Collections.Generic;
using System.Linq;
#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System.Linq.Expressions;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Extensions
{
    /// <summary>
    /// Expression externsions methods.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Converts an expression to an expression node.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="factorySettings">The factory settings to use.</param>
        /// <returns></returns>
        public static ExpressionNode ToExpressionNode(this Expression expression, FactorySettings factorySettings = null)
        {
            var converter = new ExpressionConverter();
            return converter.Convert(expression, factorySettings);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Converts an expression to an json encoded string.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="factorySettings">The factory settings to use.</param>
        /// <returns></returns>
        public static string ToJson(this Expression expression, FactorySettings factorySettings = null)
        {
            return expression.ToJson(expression.GetDefaultFactory(factorySettings));
        }
        
        /// <summary>
        /// Converts an expression to an json encoded string using the given factory.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        public static string ToJson(this Expression expression, INodeFactory factory)
        {
            return expression.ToJson(factory, new JsonSerializer());
        }
#endif

        /// <summary>
        /// Converts an expression to an json encoded string using the given factory and serializer.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public static string ToJson(this Expression expression, INodeFactory factory, IJsonSerializer serializer)
        {
            return expression.ToText(factory, serializer);
        }

        /// <summary>
        /// Converts an expression to an xml encoded string.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="factorySettings">The factory settings to use.</param>
        /// <returns></returns>
        public static string ToXml(this Expression expression, FactorySettings factorySettings = null)
        {
            return expression.ToXml(expression.GetDefaultFactory(factorySettings));
        }

        /// <summary>
        /// Converts an expression to an xml encoded string using the given factory.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        public static string ToXml(this Expression expression, INodeFactory factory)
        {
            return expression.ToXml(factory, new XmlSerializer());
        }

        /// <summary>
        /// Converts an expression to an xml encoded string using the given factory and serializer.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public static string ToXml(this Expression expression, INodeFactory factory, IXmlSerializer serializer)
        {
            return expression.ToText(factory, serializer);
        }

        /// <summary>
        /// Converts an expression to an encoded string using the given factory and serializer.
        /// The encoding is decided by the serializer.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// factory
        /// or
        /// serializer
        /// </exception>
        public static string ToText(this Expression expression, INodeFactory factory, ITextSerializer serializer)
        {            
            if(factory == null)
                throw new ArgumentNullException("factory");
            if(serializer == null)
                throw new ArgumentNullException("serializer");

            return serializer.Serialize(factory.Create(expression));
        }

        /// <summary>
        /// Gets the default factory.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="factorySettings">The factory settings to use.</param>
        /// <returns></returns>
        internal static INodeFactory GetDefaultFactory(this Expression expression, FactorySettings factorySettings)
        {
            var lambda = expression as LambdaExpression;
            if(lambda != null)
                return  new DefaultNodeFactory(lambda.Parameters.Select(p => p.Type), factorySettings);
            return new NodeFactory(factorySettings);
        }

        /// <summary>
        /// Gets the link nodes of an expression tree.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        internal static IEnumerable<Expression> GetLinkNodes(this Expression expression)
        {
            if (expression is LambdaExpression)
            {
                var lambdaExpression = (LambdaExpression)expression;

                yield return lambdaExpression.Body;
                foreach (var parameter in lambdaExpression.Parameters)
                    yield return parameter;
            }
            else if (expression is BinaryExpression)
            {
                var binaryExpression = (BinaryExpression)expression;

                yield return binaryExpression.Left;
                yield return binaryExpression.Right;
            }
            else if (expression is ConditionalExpression)
            {
                var conditionalExpression = (ConditionalExpression)expression;

                yield return conditionalExpression.IfTrue;
                yield return conditionalExpression.IfFalse;
                yield return conditionalExpression.Test;
            }
            else if (expression is InvocationExpression)
            {
                var invocationExpression = (InvocationExpression)expression;
                yield return invocationExpression.Expression;
                foreach (var argument in invocationExpression.Arguments)
                    yield return argument;                
            }
            else if (expression is ListInitExpression)
            {
                yield return (expression as ListInitExpression).NewExpression;
            }
            else if (expression is MemberExpression)
            {
                yield return (expression as MemberExpression).Expression;
            }
            else if (expression is MemberInitExpression)
            {
                yield return (expression as MemberInitExpression).NewExpression;
            }
            else if (expression is MethodCallExpression)
            {
                var methodCallExpression = (MethodCallExpression)expression;
                foreach (var argument in methodCallExpression.Arguments)
                    yield return argument;
                if (methodCallExpression.Object != null)                
                    yield return methodCallExpression.Object;                
            }
            else if (expression is NewArrayExpression)
            {
                foreach (var item in (expression as NewArrayExpression).Expressions)
                    yield return item;
            }
            else if (expression is NewExpression)
            {
                foreach (var item in (expression as NewExpression).Arguments)
                    yield return item;
            }
            else if (expression is TypeBinaryExpression)
            {
                yield return (expression as TypeBinaryExpression).Expression;
            }
            else if (expression is UnaryExpression)
            {
                yield return (expression as UnaryExpression).Operand;
            }
        }

        /// <summary>
        /// Gets the nodes of an expression tree.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        internal static IEnumerable<Expression> GetNodes(this Expression expression)
        {
            foreach (var node in expression.GetLinkNodes())
            {
                foreach (var subNode in node.GetNodes())
                    yield return subNode;
            }
            yield return expression;
        }

        /// <summary>
        /// Gets the nodes of an expression tree of given expression type.
        /// </summary>
        /// <typeparam name="TExpression">The type of the expression.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        internal static IEnumerable<TExpression> GetNodes<TExpression>(this Expression expression) where TExpression : Expression
        {
            return expression.GetNodes().OfType<TExpression>();
        }
    }
}