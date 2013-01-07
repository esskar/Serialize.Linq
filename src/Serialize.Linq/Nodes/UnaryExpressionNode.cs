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
    [DataContract(Name = "U")]
#endif
    #endregion
    public class UnaryExpressionNode : ExpressionNode<UnaryExpression>
    {
        public UnaryExpressionNode() { }

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

        public override Expression ToExpression(ExpressionContext context)
        {
            return this.NodeType == ExpressionType.UnaryPlus
                ? Expression.UnaryPlus(this.Operand.ToExpression(context))
                : Expression.MakeUnary(this.NodeType, this.Operand.ToExpression(context), this.Type.ToType(context));
        }
    }
}
