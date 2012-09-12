using System;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class RefTypeNode : TypeNode
    {
        public RefTypeNode(INodeFactory factory, int refType)
            : base(factory)
        {
            this.Ref = refType;
        }

        [DataMember]        
        public int Ref { get; set; }

        public override Type ToType()
        {
            return this.Ref >= 0 ? this.Factory.ResolveTypeRef(this.Ref) : null;
        }
    }
}