using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public abstract class MemberBindingNode : Node
    {
        protected MemberBindingNode(IExpressionNodeFactory factory) 
            : base(factory) { }

        protected MemberBindingNode(IExpressionNodeFactory factory, MemberBindingType bindingType, MemberInfo memberInfo) 
            : base(factory) 
        {
            this.BindingType = bindingType;
            this.Member = memberInfo;
        }

        [DataMember]
        public MemberBindingType BindingType { get; set; }

        [IgnoreDataMember]
        public MemberInfo Member { get; set; }

        [DataMember]
        public string MemberName
        {
            get { return SerializationHelper.SerializeMember(this.Member, this.Factory.UseAssemblyQualifiedName); }
            set { this.Member = SerializationHelper.DeserializeMember(value); }
        }

        public abstract MemberBinding ToMemberBinding();

        internal static MemberBindingNode Create(IExpressionNodeFactory factory, MemberBinding memberBinding)
        {
            MemberBindingNode memberBindingNode = null;

            if (memberBinding is MemberAssignment)         
                memberBindingNode = new MemberAssignmentNode(factory, (MemberAssignment)memberBinding);
            else if (memberBinding is MemberListBinding)   
                memberBindingNode = new MemberListBindingNode(factory, (MemberListBinding)memberBinding);
            else if (memberBinding is MemberMemberBinding) 
                memberBindingNode = new MemberMemberBindingNode(factory, (MemberMemberBinding)memberBinding);
            else if(memberBinding != null)
                throw new ArgumentException("Unknown member binding of type " + memberBinding.GetType(), "memberBinding");

            return memberBindingNode;
        }
    }
}
