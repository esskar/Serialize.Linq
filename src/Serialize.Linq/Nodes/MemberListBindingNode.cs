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
    [DataContract(Name = "MLB")]
#endif
    #endregion
    public class MemberListBindingNode : MemberBindingNode
    {
        public MemberListBindingNode() { }

        public MemberListBindingNode(INodeFactory factory, MemberListBinding memberListBinding)
            : base(factory, memberListBinding.BindingType, memberListBinding.Member)
        {
            this.Initializers = new ElementInitNodeList(this.Factory, memberListBinding.Initializers);
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "I")]
#endif
        #endregion
        public ElementInitNodeList Initializers { get; set; }

        internal override MemberBinding ToMemberBinding(ExpressionContext context)
        {
            return Expression.ListBind(this.Member.ToMemberInfo(), this.Initializers.GetElementInits(context));
        }
    }
}
