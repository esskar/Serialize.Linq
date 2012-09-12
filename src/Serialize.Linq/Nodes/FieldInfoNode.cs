using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class MemberInfoNode : MemberNode<MemberInfo>
    {
        public MemberInfoNode(INodeFactory factory, MemberInfo memberInfo) 
            : base(factory, memberInfo) { }

        protected override IEnumerable<MemberInfo> GetMemberInfosForType(Type type)
        {
            return type.GetMembers();
        }
    }
}