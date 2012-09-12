using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class MemberMemberBindingNode : MemberBindingNode
    {
        public MemberMemberBindingNode(IExpressionNodeFactory factory, MemberMemberBinding memberMemberBinding) 
            : base(factory, memberMemberBinding.BindingType, memberMemberBinding.Member)
        {
            this.Bindings = new MemberBindingNodeList(factory, memberMemberBinding.Bindings);
        }

        [DataMember]
        public MemberBindingNodeList Bindings { get; set; }

        public override MemberBinding ToMemberBinding()
        {
            return Expression.MemberBind(this.Member.ToMemberInfo(), this.Bindings.GetMemberBindings());
        }
    }
}
