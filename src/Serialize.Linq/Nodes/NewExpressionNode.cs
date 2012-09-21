using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "N")]
#endif
    #endregion
    public class NewExpressionNode : ExpressionNode<NewExpression>
    {
        public NewExpressionNode(INodeFactory factory, NewExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "A")]
#endif
        #endregion
        public ExpressionNodeList Arguments { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "C")]
#endif
        #endregion
        public ConstructorInfoNode Constructor { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "M")]
#endif
        #endregion
        public MemberInfoNodeList Members { get; set; }

        protected override void Initialize(NewExpression expression)
        {
            this.Arguments = new ExpressionNodeList(this.Factory, expression.Arguments);
            this.Constructor = new ConstructorInfoNode(this.Factory, expression.Constructor);
            this.Members = new MemberInfoNodeList(this.Factory, expression.Members);
        }

        public override Expression ToExpression()
        {
            return this.Constructor != null
                ? Expression.New(this.Constructor.ToMemberInfo(), this.Arguments.GetExpressions(), this.Members.GetMembers())
                : Expression.New(this.Type.ToType());
        }
    }
}
