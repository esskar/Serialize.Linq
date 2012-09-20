using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]    
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

    [DataContract]
    public abstract class ExpressionNode : Node
    {
        protected ExpressionNode(INodeFactory factory, ExpressionType nodeType, Type type = null)
            : base(factory)
        {
            this.NodeType = nodeType;
            this.Type = new TypeNode(factory, type);
        }

        [DataMember]
        public ExpressionType NodeType { get; set; }        

        [DataMember]
        public virtual TypeNode Type { get; set; }
        
        public abstract Expression ToExpression();

        public override string ToString()
        {
            return this.ToExpression().ToString();
        }        
    }    
}
