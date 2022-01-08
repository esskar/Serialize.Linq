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
    [DataContract(Name = "MC")]   
#endif
#if !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class MethodCallExpressionNode : ExpressionNode<MethodCallExpression>
    {
        public MethodCallExpressionNode() { }

        public MethodCallExpressionNode(INodeFactory factory, MethodCallExpression expression)
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
        [DataMember(EmitDefaultValue = false, Name = "M")]
#endif
        #endregion
        public MethodInfoNode Method { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "O")]
#endif
        #endregion
        public ExpressionNode Object { get; set; }

        protected override void Initialize(MethodCallExpression expression)
        {
            Arguments = new ExpressionNodeList(Factory, expression.Arguments);
            Method = new MethodInfoNode(Factory, expression.Method);
            Object = Factory.Create(expression.Object);
        }

        public override Expression ToExpression(IExpressionContext context)
        {
            Expression objectExpression = null;
            if (Object != null)
                objectExpression = Object.ToExpression(context);

            return Expression.Call(objectExpression, Method.ToMemberInfo(context), Arguments.GetExpressions(context).ToArray());
        }
    }
}
