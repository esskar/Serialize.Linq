using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class UnaryExpressionNode : ExpressionNode<UnaryExpression>
    {
        public UnaryExpressionNode(UnaryExpression expression) 
            : base(expression) { }

        public UnaryExpressionNode(IExpressionNodeFactory factory, UnaryExpression expression) 
            : base(factory, expression) { }

        [DataMember]
        public ExpressionNode Operand { get; set; }

        protected override void Initialize(UnaryExpression expression)
        {
            this.Operand = this.Factory.Create(expression.Operand);
        }

        public override Expression ToExpression()
        {
            return this.NodeType == ExpressionType.UnaryPlus
                ? Expression.UnaryPlus(this.Operand.ToExpression())
                : Expression.MakeUnary(this.NodeType, this.Operand.ToExpression(), this.Type.ToType());
        }
    }
}
