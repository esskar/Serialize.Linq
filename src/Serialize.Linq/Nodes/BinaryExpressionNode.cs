using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class BinaryExpressionNode : ExpressionNode<BinaryExpression>
    {
        public BinaryExpressionNode(INodeFactory factory, BinaryExpression expression) 
            : base(factory, expression) { }

        [DataMember]
        public ExpressionNode Conversion { get; set; }

        [DataMember]
        public bool IsLiftedToNull { get; set; }

        [DataMember]
        public ExpressionNode Left { get; set; }

        [DataMember]
        public MethodInfoNode Method { get; set; }
        
        [DataMember]
        public ExpressionNode Right { get; set; }

        protected override void Initialize(BinaryExpression expression)
        {
            this.Left = this.Factory.Create(expression.Left);
            this.Right = this.Factory.Create(expression.Right);
            this.Conversion = this.Factory.Create(expression.Conversion);
            this.Method = new MethodInfoNode(this.Factory, expression.Method);
            this.IsLiftedToNull = expression.IsLiftedToNull;
        }

        public override Expression ToExpression()
        {
            var conversion = this.Conversion != null ? this.Conversion.ToExpression() as LambdaExpression : null;
            if (this.Method != null && conversion != null)
                return Expression.MakeBinary(
                    this.NodeType,
                    this.Left.ToExpression(), this.Right.ToExpression(),
                    this.IsLiftedToNull,
                    this.Method.ToMemberInfo(),
                    conversion);
            if (this.Method != null)
                return Expression.MakeBinary(
                    this.NodeType,
                    this.Left.ToExpression(), this.Right.ToExpression(),
                    this.IsLiftedToNull,
                    this.Method.ToMemberInfo());
            return Expression.MakeBinary(this.NodeType,
                    this.Left.ToExpression(), this.Right.ToExpression());
        }        
    }
}
