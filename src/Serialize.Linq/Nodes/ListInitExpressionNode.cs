using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "LI")]
#endif
    #endregion
    public class ListInitExpressionNode : ExpressionNode<ListInitExpression>
    {
        public ListInitExpressionNode() { }

        public ListInitExpressionNode(INodeFactory factory, ListInitExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "I")]
#endif
        #endregion
        public ElementInitNodeList Initializers { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "N")]
#endif
        #endregion
        public ExpressionNode NewExpression { get; set; }

        protected override void Initialize(ListInitExpression expression)
        {
            this.Initializers = new ElementInitNodeList(this.Factory, expression.Initializers);
            this.NewExpression = this.Factory.Create(expression);
        }

        public override Expression ToExpression(ExpressionContext context)
        {
            return Expression.ListInit((NewExpression)this.NewExpression.ToExpression(context), this.Initializers.GetElementInits(context));
        }
    }
}
