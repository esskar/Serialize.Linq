using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "MIE")]
#endif
    #endregion
    public class MemberInitExpressionNode : ExpressionNode<MemberInitExpression>
    {
        public MemberInitExpressionNode(INodeFactory factory, MemberInitExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [XmlIgnore]
#endif
        #endregion
        public MemberBindingNodeList Bindings { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "N")]
#endif
        #endregion
        public NewExpressionNode NewExpression { get; set; }

        protected override void Initialize(MemberInitExpression expression)
        {
            this.Bindings = new MemberBindingNodeList(this.Factory, expression.Bindings);
            this.NewExpression = (NewExpressionNode)this.Factory.Create(expression.NewExpression);
        }

        public override Expression ToExpression()
        {
            return Expression.MemberInit((NewExpression)this.NewExpression.ToExpression(), this.Bindings.GetMemberBindings());
        }
    }
}
