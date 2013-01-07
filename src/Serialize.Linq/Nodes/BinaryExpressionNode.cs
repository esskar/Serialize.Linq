using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "B")]
#endif
    #endregion
    public class BinaryExpressionNode : ExpressionNode<BinaryExpression>
    {
        public BinaryExpressionNode() { }

        public BinaryExpressionNode(INodeFactory factory, BinaryExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "C")]
#endif
        #endregion
        public ExpressionNode Conversion { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "I")]
#endif
        #endregion
        public bool IsLiftedToNull { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "L")]
#endif
        #endregion
        public ExpressionNode Left { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "M")]
#endif
        #endregion
        public MethodInfoNode Method { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "R")]
#endif
        #endregion
        public ExpressionNode Right { get; set; }

        protected override void Initialize(BinaryExpression expression)
        {
            this.Left = this.Factory.Create(expression.Left);
            this.Right = this.Factory.Create(expression.Right);
            this.Conversion = this.Factory.Create(expression.Conversion);
            this.Method = new MethodInfoNode(this.Factory, expression.Method);
            this.IsLiftedToNull = expression.IsLiftedToNull;
        }

        internal override Expression ToExpression(ExpressionContext context)
        {
            var conversion = this.Conversion != null ? this.Conversion.ToExpression() as LambdaExpression : null;
            if (this.Method != null && conversion != null)
                return Expression.MakeBinary(
                    this.NodeType,
                    this.Left.ToExpression(context), this.Right.ToExpression(context),
                    this.IsLiftedToNull,
                    this.Method.ToMemberInfo(context),
                    conversion);
            if (this.Method != null)
                return Expression.MakeBinary(
                    this.NodeType,
                    this.Left.ToExpression(context), this.Right.ToExpression(context),
                    this.IsLiftedToNull,
                    this.Method.ToMemberInfo(context));
            return Expression.MakeBinary(this.NodeType,
                    this.Left.ToExpression(context), this.Right.ToExpression(context));
        }
    }
}
