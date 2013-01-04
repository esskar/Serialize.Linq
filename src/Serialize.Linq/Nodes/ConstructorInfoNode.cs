using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "CI")]
#endif
    #endregion
    public class ConstructorInfoNode : MemberNode<ConstructorInfo>
    {
        public ConstructorInfoNode() { }

        public ConstructorInfoNode(INodeFactory factory, ConstructorInfo memberInfo)
            : base(factory, memberInfo) { }

        protected override IEnumerable<ConstructorInfo> GetMemberInfosForType(Type type)
        {
            return type.GetConstructors();
        }
    }
}