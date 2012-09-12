using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public abstract class MemberNode<TMemberInfo> : Node where TMemberInfo: MemberInfo
    {
        protected MemberNode(INodeFactory factory, TMemberInfo memberInfo)
            : base(factory)
        {
            if(memberInfo != null)
                this.Initialize(memberInfo);            
        }

        [DataMember]
        public TypeNode DeclaringType { get; set; }

        [DataMember]
        public MemberTypes MemberType { get; set; }

        [DataMember]
        public string Signature { get; set; }

        protected virtual void Initialize(TMemberInfo memberInfo)
        {                        
            this.DeclaringType = this.Factory.Create(memberInfo.DeclaringType);
            this.MemberType = memberInfo.MemberType;
            this.Signature = memberInfo.ToString();
        }

        protected Type GetDeclaringType()
        {
            if(this.DeclaringType == null)
                throw new InvalidOperationException("DeclaringType is not set.");

            var declaringType = this.DeclaringType.ToType();
            if(declaringType == null)
                throw new TypeLoadException("Failed to load DeclaringType: " + this.DeclaringType);

            return declaringType;
        }

        protected abstract IEnumerable<TMemberInfo> GetMemberInfosForType(Type type);

        public virtual TMemberInfo ToMemberInfo()
        {
            if (string.IsNullOrWhiteSpace(this.Signature))
                return null;

            return this.GetMemberInfosForType(this.GetDeclaringType()).First(m => m.ToString() == this.Signature);
        }
    }
}