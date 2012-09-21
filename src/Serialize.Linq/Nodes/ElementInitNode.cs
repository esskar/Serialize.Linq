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
    [DataContract(Name = "EI")]
#endif
    #endregion
    public class ElementInitNode : Node
    {
        public ElementInitNode(INodeFactory factory, ElementInit elementInit)
            : base(factory)
        {
            this.Initialize(elementInit);
        }

        private void Initialize(ElementInit elementInit)
        {
            if (elementInit == null)
                throw new ArgumentNullException("elementInit");

            this.AddMethod = new MethodInfoNode(this.Factory, elementInit.AddMethod);
            this.Arguments = new ExpressionNodeList(this.Factory, elementInit.Arguments);
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "A")]
#endif
        #endregion
        public ExpressionNodeList Arguments { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember]
#else
        [DataMember(Name = "M")]
#endif
        #endregion
        public MethodInfoNode AddMethod { get; set; }

        public ElementInit ToElementInit()
        {
            return Expression.ElementInit(this.AddMethod.ToMemberInfo(), this.Arguments.GetExpressions());
        }
    }
}
