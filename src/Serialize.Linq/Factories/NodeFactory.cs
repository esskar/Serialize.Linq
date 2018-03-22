#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Linq.Expressions;
using System.Reflection;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public class NodeFactory : INodeFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeFactory"/> class.
        /// </summary>
        public NodeFactory()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeFactory"/> class.
        /// </summary>
        /// <param name="factorySettings">The factory settings to use.</param>
        public NodeFactory(FactorySettings factorySettings)
        {
            Settings = factorySettings ?? new FactorySettings();
        }

        public FactorySettings Settings { get; private set; }

        /// <summary>
        /// Creates an expression node from an expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Unknown expression of type  + expression.GetType()</exception>
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

        /// <summary>
        /// Creates an type node from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public TypeNode Create(Type type)
        {
            return new TypeNode(this, type);
        }

        /// <summary>
        /// Gets binding flags to be used when accessing type members.
        /// </summary>
        public BindingFlags? GetBindingFlags()
        {
            if (!this.Settings.AllowPrivateFieldAccess)
                return null;

            return BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        }
    }
}