using System;
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
    [Serializable]
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
            Operand = Factory.Create(expression.Operand);
        }

        public override Expression ToExpression(IExpressionContext context)
        {
            return NodeType == ExpressionType.UnaryPlus
                ? Expression.UnaryPlus(Operand.ToExpression(context))
                : Expression.MakeUnary(NodeType, Operand.ToExpression(context), Type.ToType(context));
        }
    }
}
