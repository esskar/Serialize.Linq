﻿#region Copyright
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
    [DataContract(Name = "IF")]   
#endif
#if !SILVERLIGHT && !NETCOREAPP1_1 && !NETSTANDARD1_6 && !NETCOREAPP2_0 && !NETFX_CORE && !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class ConditionalExpressionNode : ExpressionNode<ConditionalExpression>
    {
        public ConditionalExpressionNode() { }

        public ConditionalExpressionNode(INodeFactory factory, ConditionalExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "IFF")]
#endif
        #endregion
        public ExpressionNode IfFalse { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "IFT")]
#endif
        #endregion
        public ExpressionNode IfTrue { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "C")]
#endif
        #endregion
        public ExpressionNode Test { get; set; }

        /// <summary>
        /// Initializes the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected override void Initialize(ConditionalExpression expression)
        {
            this.Test = this.Factory.Create(expression.Test);
            this.IfTrue = this.Factory.Create(expression.IfTrue);
            this.IfFalse = this.Factory.Create(expression.IfFalse);
        }

        /// <summary>
        /// Converts this instance to an expression.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override Expression ToExpression(IExpressionContext context)
        {
            return Expression.Condition(this.Test.ToExpression(context), this.IfTrue.ToExpression(context), this.IfFalse.ToExpression(context));
        }
    }
}
