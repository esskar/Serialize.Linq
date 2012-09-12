using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class BinaryExpressionNode : ExpressionNode<BinaryExpression>
    {
        public BinaryExpressionNode(BinaryExpression expression) 
            : base(expression) { }

        public BinaryExpressionNode(IExpressionNodeFactory factory, BinaryExpression expression) 
            : base(factory, expression) { }

        [DataMember]
        public ExpressionNode Conversion { get; set; }

        [DataMember]
        public bool IsLiftedToNull { get; set; }

        [DataMember]
        public ExpressionNode Left { get; set; }

        [IgnoreDataMember]
        public MethodInfo Method { get; set; }

        [DataMember]
        public string MethodName
        {
            get { return SerializationHelper.SerializeMethod(this.Method); }
            set { this.Method = SerializationHelper.DeserializeMethod(value); }
        }

        [DataMember]
        public ExpressionNode Right { get; set; }

        protected override void Initialize(BinaryExpression expression)
        {
            this.Left = this.Factory.Create(expression.Left);
            this.Right = this.Factory.Create(expression.Right);
            this.Conversion = this.Factory.Create(expression.Conversion);
            this.Method = expression.Method;
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
                    this.Method,
                    conversion);
            if (this.Method != null)
                return Expression.MakeBinary(
                    this.NodeType,
                    this.Left.ToExpression(), this.Right.ToExpression(),
                    this.IsLiftedToNull,
                    this.Method);
            return Expression.MakeBinary(this.NodeType,
                    this.Left.ToExpression(), this.Right.ToExpression());
        }        
    }
}
