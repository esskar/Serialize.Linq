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
    [DataContract(Name = "I")]
#endif
#if !SILVERLIGHT
    [Serializable]
#endif
    #endregion
    public class InvocationExpressionNode : ExpressionNode<InvocationExpression>
    {
        public InvocationExpressionNode() { }

        public InvocationExpressionNode(INodeFactory factory, InvocationExpression expression)
            : base(factory, expression) { }

        #region DataMember
        #if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "A")]
#endif
        #endregion
        public ExpressionNodeList Arguments { get; set; }

        #region DataMember
        #if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "E")]
#endif
        #endregion
        public ExpressionNode Expression { get; set; }

        protected override void Initialize(InvocationExpression expression)
        {
            this.Arguments = new ExpressionNodeList(this.Factory, expression.Arguments);
            this.Expression = this.Factory.Create(expression.Expression);
        }

        public override Expression ToExpression(ExpressionContext context)
        {
            return System.Linq.Expressions.Expression.Invoke(this.Expression.ToExpression(context), this.Arguments.GetExpressions(context));
        }
    }
}
