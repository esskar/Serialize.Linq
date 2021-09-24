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
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region CollectionDataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [CollectionDataContract]
#else
    [CollectionDataContract(Name = "EIL")]
#endif
#if !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class ElementInitNodeList
    {
        private readonly IEnumerable<ElementInitNode> _items;

        public ElementInitNodeList()
        {
            _items = new List<ElementInitNode>();
        }

        public ElementInitNodeList(INodeFactory factory, IEnumerable<ElementInit> items)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            _items = items.Select(item => new ElementInitNode(factory, item));
        }

        internal IEnumerable<ElementInit> GetElementInits(IExpressionContext context)
        {
            return _items.Select(item => item.ToElementInit(context));
        }
    }
}
