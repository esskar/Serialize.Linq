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
    [CollectionDataContract(Name = "EL")]
#endif
    #endregion
    public class ExpressionNodeList : List<ExpressionNode>
    {
        public ExpressionNodeList() { }

        public ExpressionNodeList(INodeFactory factory, IEnumerable<Expression> items)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (items == null)
                throw new ArgumentNullException("items");
            this.AddRange(items.Select(factory.Create));
        }

        public IEnumerable<Expression> GetExpressions()
        {
            return this.Select(e => e.ToExpression());
        }

        public IEnumerable<ParameterExpression> GetParameterExpressions()
        {
            return this.OfType<ParameterExpressionNode>().Select(e => (ParameterExpression)e.ToExpression());
        }
    }
}