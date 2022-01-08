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
#if !WINDOWS_UWP
    [Serializable]
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
            Expressions = new ExpressionNodeList(Factory, expression.Expressions);
        }

        public override Expression ToExpression(IExpressionContext context)
        {
            switch (NodeType)
            {
                case ExpressionType.NewArrayBounds:
                    return Expression.NewArrayBounds(Type.ToType(context).GetElementType(), Expressions.GetExpressions(context));

                case ExpressionType.NewArrayInit:
                    return Expression.NewArrayInit(Type.ToType(context).GetElementType(), Expressions.GetExpressions(context));

                default:
                    throw new InvalidOperationException("Unhandeled nody type: " + NodeType);
            }
        }
    }
}
