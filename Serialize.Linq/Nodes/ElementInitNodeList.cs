using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Factories;

namespace Serialize.Linq.Nodes
{
    [CollectionDataContract]
    public class ElementInitNodeList : List<ElementInitNode>
    {
        public ElementInitNodeList(IExpressionNodeFactory factory, IEnumerable<ElementInit> items)            
        {
            if(factory == null)
                throw new ArgumentNullException("factory");
            if(items == null)
                throw new ArgumentNullException("items");
            this.AddRange(items.Select(item => new ElementInitNode(factory, item)));
        }

        public IEnumerable<ElementInit> GetElementInits()
        {
            return this.Select(item => item.ToElementInit());
        }
    }
}
