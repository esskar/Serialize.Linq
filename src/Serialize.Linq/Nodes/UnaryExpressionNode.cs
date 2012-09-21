using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "U")]
#endif
    #endregion
    public class UnaryExpressionNode : ExpressionNode<UnaryExpression>
    {
        public UnaryExpressionNode(INodeFactory factory, UnaryExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "O")]
#endif
        #endregion
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
