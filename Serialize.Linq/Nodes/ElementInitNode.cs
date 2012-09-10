using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Factories;
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

            this.AddMethod = elementInit.AddMethod;
            this.Arguments = new ExpressionNodeList(this.Factory, elementInit.Arguments);
        }

        [DataMember]
        public ExpressionNodeList Arguments { get; set; }

        [IgnoreDataMember]
        public MethodInfo AddMethod { get; set; }

        [DataMember]
        public string AddMethodName
        {
            get { return SerializationHelper.SerializeMethod(this.AddMethod); }
            set { this.AddMethod = SerializationHelper.DeserializeMethod(value); }
        }

        public ElementInit ToElementInit()
        {
            return Expression.ElementInit(this.AddMethod, this.Arguments.GetExpressions());
        }
    }
}
