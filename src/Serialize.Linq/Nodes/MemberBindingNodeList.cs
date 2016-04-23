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
#if !(SILVERLIGHT || DNXCORE50 || DOTNET5_4)
    [Serializable]
#endif
    #endregion
    public class MemberBindingNodeList : List<MemberBindingNode>
    {
        public MemberBindingNodeList() { }

        public MemberBindingNodeList(INodeFactory factory, IEnumerable<MemberBinding> items)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (items == null)
                throw new ArgumentNullException("items");
            this.AddRange(items.Select(m => MemberBindingNode.Create(factory, m)));
        }

        internal IEnumerable<MemberBinding> GetMemberBindings(ExpressionContext context)
        {
            return this.Select(memberBindingEntity => memberBindingEntity.ToMemberBinding(context));
        }
    }
}
