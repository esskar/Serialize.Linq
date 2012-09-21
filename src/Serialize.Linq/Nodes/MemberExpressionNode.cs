using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "M")]
#endif
    #endregion
    public class MemberExpressionNode : ExpressionNode<MemberExpression>
    {
        public MemberExpressionNode(INodeFactory factory, MemberExpression expression)
            : base(factory, expression) { }

        #region DataMember
        #if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "E")]
#endif
        #endregion
        public ExpressionNode Expression { get; set; }

        #region DataMember
        #if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "M")]
#endif
        #endregion
        public MemberInfoNode Member { get; set; }

        protected override void Initialize(MemberExpression expression)
        {
            this.Expression = this.Factory.Create(expression.Expression);
            this.Member = new MemberInfoNode(this.Factory, expression.Member);
        }

        public override Expression ToExpression()
        {
            return System.Linq.Expressions.Expression.MakeMemberAccess(this.Expression.ToExpression(), this.Member.ToMemberInfo());
        }
    }
}
