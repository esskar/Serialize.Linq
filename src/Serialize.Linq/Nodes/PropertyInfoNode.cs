using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class PropertyInfoNode : MemberNode<PropertyInfo>
    {
        public PropertyInfoNode(IExpressionNodeFactory factory, PropertyInfo memberInfo) 
            : base(factory, memberInfo) { }

        protected override IEnumerable<PropertyInfo> GetMemberInfosForType(Type type)
        {
            return type.GetProperties();
        }
    }
}