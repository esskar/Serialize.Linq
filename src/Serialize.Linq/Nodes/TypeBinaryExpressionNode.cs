#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

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
    [DataContract(Name = "TB")]   
#endif
#if !SILVERLIGHT && !NETSTANDARD && !NETFX_CORE && !WINDOWS_UWP
    [Serializable]
#endif

    #endregion
    public class TypeBinaryExpressionNode : ExpressionNode<TypeBinaryExpression>
    {
        public TypeBinaryExpressionNode() { }

        public TypeBinaryExpressionNode(INodeFactory factory, TypeBinaryExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "E")]
#endif
        #endregion
        public ExpressionNode Expression { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "O")]
#endif
        #endregion
        public TypeNode TypeOperand { get; set; }


        protected override void Initialize(TypeBinaryExpression expression)
        {
            this.Expression = this.Factory.Create(expression.Expression);
            this.TypeOperand = this.Factory.Create(expression.TypeOperand);
        }

        public override Expression ToExpression(IExpressionContext context)
        {
            switch (this.NodeType)
            {
                case ExpressionType.TypeIs:
                    return System.Linq.Expressions.Expression.TypeIs(this.Expression.ToExpression(context), this.TypeOperand.ToType(context));
#if !SILVERLIGHT
                case ExpressionType.TypeEqual:
                    return System.Linq.Expressions.Expression.TypeEqual(this.Expression.ToExpression(context), this.TypeOperand.ToType(context));
#endif                    
                default:
                    throw new NotSupportedException("unrecognised TypeBinaryExpression.NodeType " + Enum.GetName(typeof(ExpressionType), this.NodeType));
            }
        }
    }
}
