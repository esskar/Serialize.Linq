using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "B")]
#endif
#if !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class BinaryExpressionNode : ExpressionNode<BinaryExpression>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryExpressionNode"/> class.
        /// </summary>
        public BinaryExpressionNode() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryExpressionNode"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="expression">The expression.</param>
        public BinaryExpressionNode(INodeFactory factory, BinaryExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets the conversion.
        /// </summary>
        /// <value>
        /// The conversion.
        /// </value>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "C")]
#endif
        #endregion
        public ExpressionNode Conversion { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets a value indicating whether this instance is lifted to null.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is lifted to null; otherwise, <c>false</c>.
        /// </value>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "I")]
#endif
        #endregion
        public bool IsLiftedToNull { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "L")]
#endif
        #endregion
        public ExpressionNode Left { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "M")]
#endif
        #endregion
        public MethodInfoNode Method { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>
        /// The right.
        /// </value>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "R")]
#endif
        #endregion
        public ExpressionNode Right { get; set; }

        /// <summary>
        /// Initializes the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected override void Initialize(BinaryExpression expression)
        {
            Left = Factory.Create(expression.Left);
            Right = Factory.Create(expression.Right);
            Conversion = Factory.Create(expression.Conversion);
            Method = new MethodInfoNode(Factory, expression.Method);
            IsLiftedToNull = expression.IsLiftedToNull;
        }

        /// <summary>
        /// Converts this instance to an expression.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override Expression ToExpression(IExpressionContext context)
        {
            var conversion = Conversion != null ? Conversion.ToExpression() as LambdaExpression : null;
            if (Method != null && conversion != null)
                return Expression.MakeBinary(
                    NodeType,
                    Left.ToExpression(context), Right.ToExpression(context),
                    IsLiftedToNull,
                    Method.ToMemberInfo(context),
                    conversion);
            if (Method != null)
                return Expression.MakeBinary(
                    NodeType,
                    Left.ToExpression(context), Right.ToExpression(context),
                    IsLiftedToNull,
                    Method.ToMemberInfo(context));
            return Expression.MakeBinary(NodeType,
                    Left.ToExpression(context), Right.ToExpression(context));
        }
    }
}
