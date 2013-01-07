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
    [DataContract(Name = "M")]
#endif
    #endregion
    public class MemberExpressionNode : ExpressionNode<MemberExpression>
    {
        public MemberExpressionNode() { }

        public MemberExpressionNode(INodeFactory factory, MemberExpression expression)
            : base(factory, expression) { }

        #region DataMember
        #if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "E")]
#endif
        #endregion
        public ExpressionNode Expression { get; set; }

        #region DataMember
        #if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "M")]
#endif
        #endregion
        public MemberInfoNode Member { get; set; }

        protected override void Initialize(MemberExpression expression)
        {
            this.Expression = this.Factory.Create(expression.Expression);
            this.Member = new MemberInfoNode(this.Factory, expression.Member);
        }

        internal override Expression ToExpression(ExpressionContext context)
        {
            return System.Linq.Expressions.Expression.MakeMemberAccess(this.Expression.ToExpression(context), this.Member.ToMemberInfo(context));
        }
    }
}
