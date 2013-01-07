using System;
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
    [DataContract(Name = "NA")]   
#endif
    #endregion
    public class NewArrayExpressionNode : ExpressionNode<NewArrayExpression>
    {
        public NewArrayExpressionNode() { }

        public NewArrayExpressionNode(INodeFactory factory, NewArrayExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "E")]
#endif
        #endregion
        public ExpressionNodeList Expressions { get; set; }

        protected override void Initialize(NewArrayExpression expression)
        {
            this.Expressions = new ExpressionNodeList(this.Factory, expression.Expressions);
        }

        internal override Expression ToExpression(ExpressionContext context)
        {
            switch (this.NodeType)
            {
                case ExpressionType.NewArrayBounds:
                    return Expression.NewArrayBounds(this.Type.ToType(context).GetElementType(), this.Expressions.GetExpressions(context));

                case ExpressionType.NewArrayInit:
                    return Expression.NewArrayInit(this.Type.ToType(context).GetElementType(), this.Expressions.GetExpressions(context));

                default:
                    throw new InvalidOperationException("Unhandeled nody type: " + this.NodeType);
            }
        }
    }
}
