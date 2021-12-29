#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Linq;
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
#if !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class NewExpressionNode : ExpressionNode<NewExpression>
    {
        public NewExpressionNode() { }

        public NewExpressionNode(INodeFactory factory, NewExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "A")]
#endif
        #endregion
        public ExpressionNodeList Arguments { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "C")]
#endif
        #endregion
        public ConstructorInfoNode Constructor { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "M")]
#endif
        #endregion
        public MemberInfoNodeList Members { get; set; }

        protected override void Initialize(NewExpression expression)
        {
            Arguments = new ExpressionNodeList(Factory, expression.Arguments);
            Constructor = new ConstructorInfoNode(Factory, expression.Constructor);
            Members = new MemberInfoNodeList(Factory, expression.Members);
        }

        public override Expression ToExpression(IExpressionContext context)
        {
            if (Constructor == null)
                return Expression.New(Type.ToType(context));

            var constructor = Constructor.ToMemberInfo(context);
            if (constructor == null)
                return Expression.New(Type.ToType(context));

            var arguments = Arguments.GetExpressions(context).ToArray();
            var members = Members != null ? Members.GetMembers(context).ToArray() : null;
            return members != null && members.Length > 0 ? Expression.New(constructor, arguments, members) : Expression.New(constructor, arguments);
        }
    }
}
