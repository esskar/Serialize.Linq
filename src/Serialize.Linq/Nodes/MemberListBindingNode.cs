using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "MLB")]
#endif
    #endregion
    public class MemberListBindingNode : MemberBindingNode
    {
        public MemberListBindingNode(INodeFactory factory, MemberListBinding memberListBinding)
            : base(factory, memberListBinding.BindingType, memberListBinding.Member)
        {
            this.Initializers = new ElementInitNodeList(this.Factory, memberListBinding.Initializers);
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "I")]
#endif
        #endregion
        public ElementInitNodeList Initializers { get; set; }

        public override MemberBinding ToMemberBinding()
        {
            return Expression.ListBind(this.Member.ToMemberInfo(), this.Initializers.GetElementInits());
        }
    }
}
