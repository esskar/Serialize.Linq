using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class ListInitExpressionNode : ExpressionNode<ListInitExpression>
    {
        public ListInitExpressionNode(ListInitExpression expression)
            : base(expression) {}

        public ListInitExpressionNode(INodeFactory factory, ListInitExpression expression)
            : base(factory, expression) { }

        [DataMember]
        public ElementInitNodeList Initializers { get; set; }

        [DataMember]
        public ExpressionNode NewExpression { get; set; }

        protected override void Initialize(ListInitExpression expression)
        {
            this.Initializers = new ElementInitNodeList(this.Factory, expression.Initializers);
            this.NewExpression = this.Factory.Create(expression);
        }

        public override Expression ToExpression()
        {
            return Expression.ListInit((NewExpression)this.NewExpression.ToExpression(), this.Initializers.GetElementInits());
        }                
    }
}
