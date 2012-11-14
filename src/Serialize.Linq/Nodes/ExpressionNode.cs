using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "tE")]    
#endif
    #endregion
    public abstract class ExpressionNode<TExpression> : ExpressionNode where TExpression : Expression
    {
        protected ExpressionNode(INodeFactory factory, TExpression expression)
            : base(factory, expression.NodeType, expression.Type)
        {
            this.Initialize(expression);
        }

        protected ExpressionNode(INodeFactory factory, ExpressionType nodeType, Type type = null)
            : base(factory, nodeType, type) { }

        protected abstract void Initialize(TExpression expression);
    }

    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "E")]
#endif
    #endregion
    public abstract class ExpressionNode : Node
    {
        protected ExpressionNode(INodeFactory factory, ExpressionType nodeType, Type type = null)
            : base(factory)
        {
            this.NodeType = nodeType;
            this.Type = new TypeNode(factory, type);
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "NT")]
#endif
        #endregion
        public ExpressionType NodeType { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "T")]
#endif
        #endregion
        public virtual TypeNode Type { get; set; }

        internal abstract Expression ToExpression(ExpressionContext context);

        public Expression ToExpression()
        {
            return this.ToExpression(new ExpressionContext());
        }

        public Expression<TDelegate> ToExpression<TDelegate>()
        {
            var expression = this.ToExpression();
            return (Expression<TDelegate>)expression;
        }

        public override string ToString()
        {
            return this.ToExpression().ToString();
        }
    }
}
