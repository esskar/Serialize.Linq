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
    [DataContract(Name = "NA")]   
#endif
    #endregion
    public class NewArrayExpressionNode : ExpressionNode<NewArrayExpression>
    {
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

        public override Expression ToExpression()
        {
            switch (this.NodeType)
            {
                case ExpressionType.NewArrayBounds:
                    return Expression.NewArrayBounds(this.Type.ToType().GetElementType(), this.Expressions.GetExpressions());

                case ExpressionType.NewArrayInit:
                    return Expression.NewArrayInit(this.Type.ToType().GetElementType(), this.Expressions.GetExpressions());

                default:
                    throw new InvalidOperationException("Unhandeled nody type: " + this.NodeType);
            }
        }
    }
}
