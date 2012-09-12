using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class MemberExpressionNode : ExpressionNode<MemberExpression>
    {   
        public MemberExpressionNode(MemberExpression expression)
            : base(expression) {}
        
        public MemberExpressionNode(INodeFactory factory, MemberExpression expression)
            : base(factory,expression) { }

        [DataMember]
        public ExpressionNode Expression { get; set; }

        [DataMember]
        public MemberInfoNode Member { get; set; }        

        protected override void Initialize(MemberExpression expression)
        {
            this.Expression = this.Factory.Create(expression.Expression);
            this.Member = new MemberInfoNode(this.Factory, expression.Member);
        }

        public override Expression ToExpression()
        {            
            return System.Linq.Expressions.Expression.MakeMemberAccess(this.Expression.ToExpression(), this.Member.ToMemberInfo());
        }
    }
}
