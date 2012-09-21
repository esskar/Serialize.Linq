using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "MB")]
#endif
    #endregion
    public abstract class MemberBindingNode : Node
    {
        protected MemberBindingNode(INodeFactory factory)
            : base(factory) { }

        protected MemberBindingNode(INodeFactory factory, MemberBindingType bindingType, MemberInfo memberInfo)
            : base(factory)
        {
            this.BindingType = bindingType;
            this.Member = new MemberInfoNode(this.Factory, memberInfo);
        }
        
        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "BT")]
#endif
        #endregion
        public MemberBindingType BindingType { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "M")]
#endif
        #endregion
        public MemberInfoNode Member { get; set; }

        public abstract MemberBinding ToMemberBinding();

        internal static MemberBindingNode Create(INodeFactory factory, MemberBinding memberBinding)
        {
            MemberBindingNode memberBindingNode = null;

            if (memberBinding is MemberAssignment)
                memberBindingNode = new MemberAssignmentNode(factory, (MemberAssignment)memberBinding);
            else if (memberBinding is MemberListBinding)
                memberBindingNode = new MemberListBindingNode(factory, (MemberListBinding)memberBinding);
            else if (memberBinding is MemberMemberBinding)
                memberBindingNode = new MemberMemberBindingNode(factory, (MemberMemberBinding)memberBinding);
            else if (memberBinding != null)
                throw new ArgumentException("Unknown member binding of type " + memberBinding.GetType(), "memberBinding");

            return memberBindingNode;
        }
    }
}
