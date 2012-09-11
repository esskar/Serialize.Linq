using System;
using System.Runtime.Serialization;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public abstract class Node
    {
        protected Node()
            : this(new ExpressionNodeFactory()) { }

        protected Node(IExpressionNodeFactory factory)
        {
            if(factory == null)
                throw new ArgumentNullException("factory");

            this.Factory = factory;
        }

        [IgnoreDataMember]
        public IExpressionNodeFactory Factory
        {
            get; private set;
        }
    }
}
