using System;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    #region KnownTypes
    [KnownType(typeof(NamedTypeNode))]
    [KnownType(typeof(RefTypeNode))]
    #endregion
    public abstract class TypeNode : Node
    {
        protected TypeNode(INodeFactory factory)
            : base(factory) { }

        public abstract Type ToType();

        public override string ToString()
        {
            return this.ToType().ToString();
        }
    }    
}