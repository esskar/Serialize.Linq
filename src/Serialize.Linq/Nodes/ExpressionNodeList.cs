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
    [Serializable]
    #endregion
    public class ExpressionNodeList : List<ExpressionNode>
    {
        public ExpressionNodeList() { }

        public ExpressionNodeList(INodeFactory factory, IEnumerable<Expression> items)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            AddRange(items.Select(factory.Create));
        }

        internal IEnumerable<Expression> GetExpressions(IExpressionContext context)
        {
            return this.Select(e => e.ToExpression(context));
        }

        internal IEnumerable<ParameterExpression> GetParameterExpressions(IExpressionContext context)
        {
            return this.OfType<ParameterExpressionNode>().Select(e => (ParameterExpression)e.ToExpression(context));
        }
    }
}