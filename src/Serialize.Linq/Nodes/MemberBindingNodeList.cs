using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [CollectionDataContract]    
    public class MemberBindingNodeList : List<MemberBindingNode>
    {
        public MemberBindingNodeList() { }

        public MemberBindingNodeList(INodeFactory factory, IEnumerable<MemberBinding> items)            
        {
            if(factory == null)
                throw new ArgumentNullException("factory");
            if(items == null)
                throw new ArgumentNullException("items");
            this.AddRange(items.Select(m => MemberBindingNode.Create(factory, m)));
        }

        public IEnumerable<MemberBinding> GetMemberBindings()
        {
            return this.Select(memberBindingEntity => memberBindingEntity.ToMemberBinding());
        }       
    }
}
