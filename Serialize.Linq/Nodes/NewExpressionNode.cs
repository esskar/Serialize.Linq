using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Factories;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class NewExpressionNode : ExpressionNode<NewExpression>
    {
        public NewExpressionNode(NewExpression expression)
            : base(expression) {}

        public NewExpressionNode(IExpressionNodeFactory factory, NewExpression expression)
            : base(factory, expression) { }

        [DataMember]
        public ExpressionNodeList Arguments { get; set; }

        [IgnoreDataMember]
        public ConstructorInfo Constructor { get; set; }

        [DataMember]
        public string ConstructorName
        {
            get { return SerializationHelper.SerializeConstructor(this.Constructor); }
            set { this.Constructor = SerializationHelper.DeserializeConstructor(value); }
        }

        [DataMember]
        public MemberInfoNodeList Members { get; set; }        

        protected override void  Initialize(NewExpression expression)
        {
 	        this.Arguments = new ExpressionNodeList(this.Factory, expression.Arguments);
            this.Constructor = expression.Constructor;
            this.Members = new MemberInfoNodeList(expression.Members);        
        }

        public override Expression ToExpression()
        {
            return this.Constructor != null
                ? Expression.New(this.Constructor, this.Arguments.GetExpressions(), this.Members.GetMembers())
                : Expression.New(this.Type);
        }
    }
}
