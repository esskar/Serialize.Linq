using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]   
    public class ConditionalExpressionNode : ExpressionNode<ConditionalExpression>
    {
        public ConditionalExpressionNode(INodeFactory factory, ConditionalExpression expression) 
            : base(factory, expression) { }

        [DataMember]
        public ExpressionNode IfFalse { get; set; }

        [DataMember]
        public ExpressionNode IfTrue { get; set; }

        [DataMember]
        public ExpressionNode Test { get; set; }

        protected override void Initialize(ConditionalExpression expression)
        {
            this.Test = this.Factory.Create(expression.Test);
            this.IfTrue = this.Factory.Create(expression.IfTrue);
            this.IfFalse = this.Factory.Create(expression.IfFalse);
        }

        public override Expression ToExpression()
        {
            return Expression.Condition(this.Test.ToExpression(), this.IfTrue.ToExpression(), this.IfFalse.ToExpression());
        }        
    }
}
