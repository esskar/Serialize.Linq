using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    [DataContract]   
    public class MethodCallExpressionNode : ExpressionNode<MethodCallExpression>
    {
        public MethodCallExpressionNode(MethodCallExpression expression)
            : base(expression) {}

        public MethodCallExpressionNode(IExpressionNodeFactory factory, MethodCallExpression expression)
            : base(factory, expression) { }

        [DataMember]
        public ExpressionNodeList Arguments { get; set; }
        
        [IgnoreDataMember]
        public MethodInfo Method { get; set; }

        [DataMember]
        public string MethodName
        {
            get { return  SerializationHelper.SerializeMethod(this.Method, this.Factory.UseAssemblyQualifiedName); }
            set { this.Method = SerializationHelper.DeserializeMethod(value); }
        }

        [DataMember]
        public ExpressionNode Object { get; set; }

        protected override void Initialize(MethodCallExpression expression)
        {
            this.Arguments = new ExpressionNodeList(this.Factory, expression.Arguments);
            this.Method = expression.Method;
            this.Object = this.Factory.Create(expression.Object);
        }

        public override Expression ToExpression()
        {
            Expression objectExpression = null;
            if (this.Object != null)
                objectExpression = this.Object.ToExpression();

            return Expression.Call(objectExpression, this.Method, this.Arguments.GetExpressions().ToArray());
        }
    }
}
