using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "MA")]   
#endif
    #endregion
    public class MemberAssignmentNode : MemberBindingNode
    {
        public MemberAssignmentNode(INodeFactory factory, MemberAssignment memberAssignment)
            : base(factory, memberAssignment.BindingType, memberAssignment.Member)
        {
            this.Expression = this.Factory.Create(memberAssignment.Expression);
        }

        #region DataMember
        #if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "E")]
#endif
        #endregion
        public ExpressionNode Expression { get; set; }

        public override MemberBinding ToMemberBinding()
        {
            return System.Linq.Expressions.Expression.Bind(this.Member.ToMemberInfo(), this.Expression.ToExpression());
        }
    }
}
