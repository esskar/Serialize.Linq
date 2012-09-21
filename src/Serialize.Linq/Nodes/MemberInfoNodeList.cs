using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [CollectionDataContract]
#else
    [CollectionDataContract(Name = "MIL")]
#endif
    #endregion
    public class MemberInfoNodeList : List<MemberInfoNode>
    {
        public MemberInfoNodeList() { }

        public MemberInfoNodeList(INodeFactory factory, IEnumerable<MemberInfo> items)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (items == null)
                throw new ArgumentNullException("items");
            this.AddRange(items.Select(m => new MemberInfoNode(factory, m)));
        }

        public IEnumerable<MemberInfo> GetMembers()
        {
            return this.Select(m => m.ToMemberInfo());
        }
    }
}
