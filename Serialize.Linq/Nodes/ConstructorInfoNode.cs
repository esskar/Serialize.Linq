using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class ConstructorInfoNode : MemberNode<ConstructorInfo>
    {
        public ConstructorInfoNode(IExpressionNodeFactory factory, ConstructorInfo memberInfo) 
            : base(factory, memberInfo) { }

        protected override IEnumerable<ConstructorInfo> GetMemberInfosForType(Type type)
        {
            return type.GetConstructors();
        }
    }
}