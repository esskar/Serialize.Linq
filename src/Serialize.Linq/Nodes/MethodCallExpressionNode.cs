using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]   
    public class MethodCallExpressionNode : ExpressionNode<MethodCallExpression>
    {
        public MethodCallExpressionNode(MethodCallExpression expression)
            : base(expression) {}

        public MethodCallExpressionNode(INodeFactory factory, MethodCallExpression expression)
            : base(factory, expression) { }

        [DataMember]
        public ExpressionNodeList Arguments { get; set; }
        
        [DataMember]
        public MethodInfoNode Method { get; set; }
        
        [DataMember]
        public ExpressionNode Object { get; set; }

        protected override void Initialize(MethodCallExpression expression)
        {
            this.Arguments = new ExpressionNodeList(this.Factory, expression.Arguments);
            this.Method = new MethodInfoNode(this.Factory, expression.Method);
            this.Object = this.Factory.Create(expression.Object);
        }

        public override Expression ToExpression()
        {
            Expression objectExpression = null;
            if (this.Object != null)
                objectExpression = this.Object.ToExpression();

            return Expression.Call(objectExpression, this.Method.ToMemberInfo(), this.Arguments.GetExpressions().ToArray());
        }
    }
}
