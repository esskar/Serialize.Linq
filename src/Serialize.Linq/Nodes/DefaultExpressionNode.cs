using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "D")]   
#endif
    [Serializable]
    #endregion
    public class DefaultExpressionNode : ExpressionNode<DefaultExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultExpressionNode"/> class.
        /// </summary>
        public DefaultExpressionNode() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultExpressionNode"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="expression">The expression.</param>
        public DefaultExpressionNode(INodeFactory factory, DefaultExpression expression)
            : base(factory, expression) { }

        protected override void Initialize(DefaultExpression expression)
        {
           // nothing to do
        }

        public override Expression ToExpression(IExpressionContext context)
        {
            return Expression.Default(Type.ToType(context));
        }
    }
}
