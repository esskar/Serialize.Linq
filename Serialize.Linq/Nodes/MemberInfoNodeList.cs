using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    [CollectionDataContract]
    public class MemberInfoNodeList : List<string>
    {
        public MemberInfoNodeList() { }

        public MemberInfoNodeList(IEnumerable<MemberInfo> items)             
        {
            if(items == null)
                throw new ArgumentNullException("items");
            this.AddRange(items.Select(SerializationHelper.SerializeMember));
        }

        public IEnumerable<MemberInfo> GetMembers()
        {            
            return this.Select(SerializationHelper.DeserializeMember);
        }   
    }
}
