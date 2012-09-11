using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]   
    public class NewArrayExpressionNode : ExpressionNode<NewArrayExpression>
    {
        public NewArrayExpressionNode(NewArrayExpression expression)
            : base(expression) { }

        public NewArrayExpressionNode(IExpressionNodeFactory factory, NewArrayExpression expression)
            : base(factory, expression) { }
        
        [DataMember]
        public ExpressionNodeList Expressions { get; set; }

        protected override void Initialize(NewArrayExpression expression)
        {
            this.Expressions = new ExpressionNodeList(this.Factory, expression.Expressions);
        }

        public override Expression ToExpression()
        {
            switch(this.NodeType)
            {
                case ExpressionType.NewArrayBounds:
                    return Expression.NewArrayBounds(this.Type.GetElementType(), this.Expressions.GetExpressions());

                case ExpressionType.NewArrayInit:
                    return Expression.NewArrayInit(this.Type.GetElementType(), this.Expressions.GetExpressions());

                default:
                    throw new InvalidOperationException("Unhandeled nody type: " + this.NodeType);
            }
        }
    }
}
