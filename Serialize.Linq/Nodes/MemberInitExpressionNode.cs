using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Serialize.Linq.Factories;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class MemberInitExpressionNode : ExpressionNode<MemberInitExpression>
    {
        public MemberInitExpressionNode(MemberInitExpression expression)
            : base(expression) {}

        public MemberInitExpressionNode(IExpressionNodeFactory factory, MemberInitExpression expression)
            : base(factory, expression) { }

        [XmlIgnore]
        public MemberBindingNodeList Bindings { get; set; }

        [DataMember]
        public NewExpressionNode NewExpression { get; set; }

        protected override void Initialize(MemberInitExpression expression)
        {
            this.Bindings = new MemberBindingNodeList(this.Factory, expression.Bindings);
            this.NewExpression = (NewExpressionNode)this.Factory.Create(expression.NewExpression);
        }
        
        public override Expression ToExpression()
        {
            return Expression.MemberInit((NewExpression)this.NewExpression.ToExpression(), this.Bindings.GetMemberBindings());
        }
    }
}
