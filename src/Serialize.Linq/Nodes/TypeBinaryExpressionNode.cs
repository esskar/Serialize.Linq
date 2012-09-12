using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]   
    public class TypeBinaryExpressionNode : ExpressionNode<TypeBinaryExpression>
    {
        public TypeBinaryExpressionNode(TypeBinaryExpression expression)
            : base(expression) { }

        public TypeBinaryExpressionNode(IExpressionNodeFactory factory, TypeBinaryExpression expression)
            : base(factory, expression) { }

        [DataMember]
        public ExpressionNode Expression { get; set; }

        protected override void Initialize(TypeBinaryExpression expression)
        {
            this.Expression = this.Factory.Create(expression.Expression);
        }

        public override Expression ToExpression()
        {
            return System.Linq.Expressions.Expression.TypeIs(this.Expression.ToExpression(), this.Type.ToType());
        }
    }
}
