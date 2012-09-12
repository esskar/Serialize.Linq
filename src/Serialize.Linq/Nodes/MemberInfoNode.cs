using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class FieldInfoNode : MemberNode<FieldInfo>
    {
        public FieldInfoNode(IExpressionNodeFactory factory, FieldInfo memberInfo) 
            : base(factory, memberInfo) { }

        protected override IEnumerable<FieldInfo> GetMemberInfosForType(Type type)
        {
            return type.GetFields();
        }
    }
}