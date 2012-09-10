using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Factories;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class InvocationExpressionNode : ExpressionNode<InvocationExpression>
    {
        public InvocationExpressionNode(InvocationExpression expression) 
            : base(expression) { }

        public InvocationExpressionNode(IExpressionNodeFactory factory, InvocationExpression expression) 
            : base(factory, expression) { }

        [DataMember]
        public ExpressionNodeList Arguments { get; set; }

        [DataMember]
        public ExpressionNode Expression { get; set; }

        protected override void Initialize(InvocationExpression expression)
        {
            this.Arguments = new ExpressionNodeList(this.Factory, expression.Arguments);
            this.Expression = this.Factory.Create(expression.Expression);
        }

        public override Expression ToExpression()
        {
            return System.Linq.Expressions.Expression.Invoke(this.Expression.ToExpression(), this.Arguments.GetExpressions());
        }        
    }
}
