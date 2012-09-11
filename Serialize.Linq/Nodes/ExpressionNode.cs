using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    public abstract class ExpressionNode<TExpression> : ExpressionNode where TExpression : Expression
    {
        protected ExpressionNode(TExpression expression)
            : base(expression.NodeType, expression.Type) 
        { 
            this.Initialize(expression);
        }

        protected ExpressionNode(IExpressionNodeFactory factory, TExpression expression)
            : base(factory, expression.NodeType, expression.Type) 
        { 
            this.Initialize(expression);
        }

        protected ExpressionNode(ExpressionType expressionType, Type type = null)
            : base(expressionType, type) { }

        protected ExpressionNode(IExpressionNodeFactory factory, ExpressionType nodeType, Type type = null)
            : base(factory, nodeType, type) { }

        protected abstract void Initialize(TExpression expression);        
    }

    [DataContract]
    public abstract class ExpressionNode : Node
    {
        protected ExpressionNode(ExpressionType expressionType, Type type = null)
            : this(new ExpressionNodeFactory(), expressionType, type) { }
        
        protected ExpressionNode(IExpressionNodeFactory factory, ExpressionType nodeType, Type type = null)
            : base(factory)
        {
            this.NodeType = nodeType;
            this.Type = type;
        }

        [DataMember]
        public ExpressionType NodeType { get; set; }        

        [IgnoreDataMember]
        public virtual Type Type { get; set; }

        [DataMember]
        public string TypeName
        {
            get { return SerializationHelper.SerializeType(this.Type); }
            set { this.Type = SerializationHelper.DeserializeType(value); }
        }

        public abstract Expression ToExpression();

        public override string ToString()
        {
            return this.ToExpression().ToString();
        }        
    }    
}
