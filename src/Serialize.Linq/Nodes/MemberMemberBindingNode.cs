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
    [DataContract(Name = "MMB")]
#endif
    #endregion
    public class MemberMemberBindingNode : MemberBindingNode
    {
        public MemberMemberBindingNode() { }

        public MemberMemberBindingNode(INodeFactory factory, MemberMemberBinding memberMemberBinding)
            : base(factory, memberMemberBinding.BindingType, memberMemberBinding.Member)
        {
            this.Bindings = new MemberBindingNodeList(factory, memberMemberBinding.Bindings);
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "B")]
#endif
        #endregion
        public MemberBindingNodeList Bindings { get; set; }

        internal override MemberBinding ToMemberBinding(ExpressionContext context)
        {
            return Expression.MemberBind(this.Member.ToMemberInfo(), this.Bindings.GetMemberBindings(context));
        }
    }
}
