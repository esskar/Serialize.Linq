using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class ElementInitNode : Node
    {
        public ElementInitNode(ElementInit elementInit)            
        {
            this.Initialize(elementInit);
        }

        public ElementInitNode(IExpressionNodeFactory factory, ElementInit elementInit)            
            : base(factory)
        {
            this.Initialize(elementInit);
        }

        private void Initialize(ElementInit elementInit)
        {
            if(elementInit == null)
                throw new ArgumentNullException("elementInit");

            this.AddMethod = new MethodInfoNode(this.Factory, elementInit.AddMethod);
            this.Arguments = new ExpressionNodeList(this.Factory, elementInit.Arguments);
        }

        [DataMember]
        public ExpressionNodeList Arguments { get; set; }

        [DataMember]
        public MethodInfoNode AddMethod { get; set; }
        
        public ElementInit ToElementInit()
        {
            return Expression.ElementInit(this.AddMethod.ToMemberInfo(), this.Arguments.GetExpressions());
        }
    }
}
