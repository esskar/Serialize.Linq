using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if SERIALIZE_LINQ_WITH_LONG_DATA_NAMES
    [DataContract]
#else
    [DataContract(Name = "PI")]
#endif
    #endregion
    public class PropertyInfoNode : MemberNode<PropertyInfo>
    {
        public PropertyInfoNode(INodeFactory factory, PropertyInfo memberInfo) 
            : base(factory, memberInfo) { }

        protected override IEnumerable<PropertyInfo> GetMemberInfosForType(Type type)
        {
            return type.GetProperties();
        }
    }
}