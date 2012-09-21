using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "IF")]   
#endif
    #endregion
    public class ConditionalExpressionNode : ExpressionNode<ConditionalExpression>
    {
        public ConditionalExpressionNode(INodeFactory factory, ConditionalExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "IFF")]
#endif
        #endregion
        public ExpressionNode IfFalse { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "IFT")]
#endif
        #endregion
        public ExpressionNode IfTrue { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "T")]
#endif
        #endregion
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
