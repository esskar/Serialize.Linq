﻿#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

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
#if !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class MemberInfoNodeList
    {
        private readonly IEnumerable<MemberInfoNode> _items;

        public MemberInfoNodeList() 
        {
            _items = new List<MemberInfoNode>();
        }

        public MemberInfoNodeList(INodeFactory factory, IEnumerable<MemberInfo> items = null)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if(items != null)
               _items = items.Select(m => new MemberInfoNode(factory, m));
        }

        public IEnumerable<MemberInfo> GetMembers(IExpressionContext context)
        {
            return _items.Select(m => m.ToMemberInfo(context));
        }
    }
}
