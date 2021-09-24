#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region CollectionDataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [CollectionDataContract]
#else
    [CollectionDataContract(Name = "MBL")]    
#endif
#if !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class MemberBindingNodeList
    {
        private readonly IEnumerable<MemberBindingNode> _items;

        public MemberBindingNodeList() 
        {
            _items = new List<MemberBindingNode>();
        }

        public MemberBindingNodeList(INodeFactory factory, IEnumerable<MemberBinding> items)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            _items = items.Select(m => MemberBindingNode.Create(factory, m));
        }

        internal IEnumerable<MemberBinding> GetMemberBindings(IExpressionContext context)
        {
            return _items.Select(memberBindingEntity => memberBindingEntity.ToMemberBinding(context));
        }
    }
}
