using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class MemberListBindingNode : MemberBindingNode
    {
        public MemberListBindingNode(IExpressionNodeFactory factory, MemberListBinding memberListBinding)
            : base(factory, memberListBinding.BindingType, memberListBinding.Member)
        {
            this.Initializers = new ElementInitNodeList(this.Factory, memberListBinding.Initializers);
        }

        [DataMember]
        public ElementInitNodeList Initializers { get; set; }

        public override MemberBinding ToMemberBinding()
        {
            return Expression.ListBind(this.Member, this.Initializers.GetElementInits());
        }    
    }
}
