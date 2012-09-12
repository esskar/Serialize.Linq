using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]   
    public class MemberAssignmentNode : MemberBindingNode
    {
        public MemberAssignmentNode(INodeFactory factory, MemberAssignment memberAssignment) 
            : base(factory, memberAssignment.BindingType, memberAssignment.Member)
        {
            this.Expression = this.Factory.Create(memberAssignment.Expression);
        }

        public ExpressionNode Expression  { get; set; }

        public override MemberBinding ToMemberBinding()
        {
            return System.Linq.Expressions.Expression.Bind(this.Member.ToMemberInfo(), this.Expression.ToExpression());
        }
    }
}
