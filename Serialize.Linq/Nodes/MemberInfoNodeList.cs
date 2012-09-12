using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    [CollectionDataContract]
    public class MemberInfoNodeList : List<string>
    {
        public MemberInfoNodeList() { }

        public MemberInfoNodeList(IExpressionNodeFactory factory, IEnumerable<MemberInfo> items)             
        {
            if(factory == null)
                throw new ArgumentNullException("factory");
            if(items == null)
                throw new ArgumentNullException("items");
            this.AddRange(items.Select(m => SerializationHelper.SerializeMember(m, factory.UseAssemblyQualifiedName)));
        }

        public IEnumerable<MemberInfo> GetMembers()
        {            
            return this.Select(SerializationHelper.DeserializeMember);
        }   
    }
}
