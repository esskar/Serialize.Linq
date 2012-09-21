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
    #endregion
    public class ElementInitNodeList : List<ElementInitNode>
    {
        public ElementInitNodeList() { }

        public ElementInitNodeList(INodeFactory factory, IEnumerable<ElementInit> items)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (items == null)
                throw new ArgumentNullException("items");
            this.AddRange(items.Select(item => new ElementInitNode(factory, item)));
        }

        public IEnumerable<ElementInit> GetElementInits()
        {
            return this.Select(item => item.ToElementInit());
        }
    }
}
