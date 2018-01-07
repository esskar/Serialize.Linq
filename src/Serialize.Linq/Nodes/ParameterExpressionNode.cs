#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

#if !WINDOWS_PHONE7
using System;
#endif
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "P")]
#endif
#if !SILVERLIGHT && !NETSTANDARD && !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class ParameterExpressionNode : ExpressionNode<ParameterExpression>
    {
        public ParameterExpressionNode() { }

        public ParameterExpressionNode(INodeFactory factory, ParameterExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "I")]
#endif

        #endregion
        public bool IsByRef { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "N")]
#endif
        #endregion
        public string Name { get; set; }

        protected override void Initialize(ParameterExpression expression)
        {
#if !WINDOWS_PHONE7
            this.IsByRef = expression.IsByRef;
#else
            this.IsByRef = false;
#endif
            this.Name = expression.Name;
        }

        public override Expression ToExpression(IExpressionContext context)
        {
            return context.GetParameterExpression(this);
        }
    }
}
