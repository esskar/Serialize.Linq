#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE    
    #if SERIALIZE_LINQ_BORKED_VERION
    [DataContract]
    #else
    [DataContract(Name = "ExpressionNodeGeneric")]
#endif
#else
    [DataContract(Name = "tE")]    
#endif
#if !SILVERLIGHT && !NETSTANDARD && !NETFX_CORE && !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    [DebuggerDisplay("ExpressionNode")]
    public abstract class ExpressionNode<TExpression> : ExpressionNode where TExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNode{TExpression}"/> class.
        /// </summary>
        protected ExpressionNode() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNode{TExpression}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="expression">The expression.</param>
        protected ExpressionNode(INodeFactory factory, TExpression expression)
            : base(factory, expression.NodeType, expression.Type)
        {
            this.Initialize(expression);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNode{TExpression}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        protected ExpressionNode(INodeFactory factory, ExpressionType nodeType, Type type = null)
            : base(factory, nodeType, type) { }

        /// <summary>
        /// Initializes this instance using the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected abstract void Initialize(TExpression expression);
    }

    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
#else
    [DataContract(Name = "E")]
#endif
#if !SILVERLIGHT && !NETSTANDARD && !NETFX_CORE && !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public abstract class ExpressionNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNode"/> class.
        /// </summary>
        protected ExpressionNode() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNode"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        protected ExpressionNode(INodeFactory factory, ExpressionType nodeType, Type type = null)
            : base(factory)
        {
            this.NodeType = nodeType;
            this.Type = new TypeNode(factory, type);
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets the type of the node.
        /// </summary>
        /// <value>
        /// The type of the node.
        /// </value>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "NT")]
#endif
        #endregion
        public ExpressionType NodeType { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "T")]
#endif
        #endregion
        public virtual TypeNode Type { get; set; }

        /// <summary>
        /// Converts this instance to an expression.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual Expression ToExpression(IExpressionContext context)
        {
            return null;
        }

        public Expression ToExpression()
        {
            return this.ToExpression(new ExpressionContext());
        }

        /// <summary>
        /// Converts this instance to an expression.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Expression<TDelegate> ToExpression<TDelegate>(ExpressionContext context = null)
        {
            return this.ToExpression(ConvertToExpression<TDelegate>, context ?? new ExpressionContext());
        }

        /// <summary>
        /// Converts this instance to an boolean expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Expression<Func<TEntity, bool>> ToBooleanExpression<TEntity>(ExpressionContext context = null)
        {
            return this.ToExpression(ConvertToBooleanExpression<TEntity>, context ?? new ExpressionContext());
        }

        /// <summary>
        /// Converts this instance to an expression.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="conversionFunction">The conversion function.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">conversionFunction</exception>
        public Expression<TDelegate> ToExpression<TDelegate>(Func<ExpressionNode, Expression<TDelegate>> conversionFunction)
        {
            if (conversionFunction == null)
                throw new ArgumentNullException("conversionFunction");
            return conversionFunction(this);
        }

        /// <summary>
        /// Converts this instance to an expression.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="conversionFunction">The conversion function.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// Parameter <paramref name="conversionFunction"/> or <paramref name="context"/> is null.
        /// </exception>
        public Expression<TDelegate> ToExpression<TDelegate>(Func<ExpressionNode, ExpressionContext, Expression<TDelegate>> conversionFunction, ExpressionContext context)
        {
            if (conversionFunction == null)
                throw new ArgumentNullException("conversionFunction");
            if (context == null)
                throw new ArgumentNullException("context");
            return conversionFunction(this, context);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.ToExpression().ToString();
        }

        /// <summary>
        /// Converts to an expression.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="expressionNode">The expression node.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private static Expression<TDelegate> ConvertToExpression<TDelegate>(ExpressionNode expressionNode, ExpressionContext context)
        {
            var expression = expressionNode.ToExpression(context);
            return (Expression<TDelegate>)expression;
        }

        /// <summary>
        /// Converts to a boolean expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="expressionNode">The expression node.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private static Expression<Func<TEntity, bool>> ConvertToBooleanExpression<TEntity>(ExpressionNode expressionNode, ExpressionContext context)
        {
            return ConvertToExpression<Func<TEntity, bool>>(expressionNode, context);
        }
    }
}
