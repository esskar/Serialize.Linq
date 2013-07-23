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
    [DataContract(Name = "NA")]   
#endif
#if !SILVERLIGHT
    [Serializable]
#endif
    #endregion
    public class NewArrayExpressionNode : ExpressionNode<NewArrayExpression>
    {
        public NewArrayExpressionNode() { }

        public NewArrayExpressionNode(INodeFactory factory, NewArrayExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "E")]
#endif
        #endregion
        public ExpressionNodeList Expressions { get; set; }

        protected override void Initialize(NewArrayExpression expression)
        {
            this.Expressions = new ExpressionNodeList(this.Factory, expression.Expressions);
        }

        public override Expression ToExpression(ExpressionContext context)
        {
            switch (this.NodeType)
            {
                case ExpressionType.NewArrayBounds:
                    return Expression.NewArrayBounds(this.Type.ToType(context).GetElementType(), this.Expressions.GetExpressions(context));

                case ExpressionType.NewArrayInit:
                    return Expression.NewArrayInit(this.Type.ToType(context).GetElementType(), this.Expressions.GetExpressions(context));

                default:
                    throw new InvalidOperationException("Unhandeled nody type: " + this.NodeType);
            }
        }
    }
}
