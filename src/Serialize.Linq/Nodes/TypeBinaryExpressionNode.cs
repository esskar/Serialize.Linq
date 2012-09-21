using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "TB")]   
#endif
    #endregion
    public class TypeBinaryExpressionNode : ExpressionNode<TypeBinaryExpression>
    {
        public TypeBinaryExpressionNode(INodeFactory factory, TypeBinaryExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "E")]
#endif
        #endregion
        public ExpressionNode Expression { get; set; }

        protected override void Initialize(TypeBinaryExpression expression)
        {
            this.Expression = this.Factory.Create(expression.Expression);
        }

        public override Expression ToExpression()
        {
            return System.Linq.Expressions.Expression.TypeIs(this.Expression.ToExpression(), this.Type.ToType());
        }
    }
}
