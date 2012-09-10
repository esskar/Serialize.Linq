using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Factories;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class MemberExpressionNode : ExpressionNode<MemberExpression>
    {   
        public MemberExpressionNode(MemberExpression expression)
            : base(expression) {}
        
        public MemberExpressionNode(IExpressionNodeFactory factory, MemberExpression expression)
            : base(factory,expression) { }

        [DataMember]
        public ExpressionNode Expression { get; set; }

        [IgnoreDataMember]
        public MemberInfo Member { get; set; }

        [DataMember]
        public string MemberName
        {
            get { return SerializationHelper.SerializeMember(this.Member); }
            set { this.Member = SerializationHelper.DeserializeMember(value); }
        }

        protected override void Initialize(MemberExpression expression)
        {
            this.Expression = this.Factory.Create(expression.Expression);
            this.Member = expression.Member;
        }

        public override Expression ToExpression()
        {            
            return System.Linq.Expressions.Expression.MakeMemberAccess(this.Expression.ToExpression(), this.Member);
        }
    }
}
