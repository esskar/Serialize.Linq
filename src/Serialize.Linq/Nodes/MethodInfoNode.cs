using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class MethodInfoNode : MemberNode<MethodInfo>
    {
        public MethodInfoNode(INodeFactory factory, MethodInfo memberInfo) 
            : base(factory, memberInfo) { }

        protected override IEnumerable<MethodInfo> GetMemberInfosForType(Type type)
        {
            return type.GetMethods();
        }

        [DataMember]
        public bool IsGenericMethod { get; set; }

        [DataMember]
        public TypeNode[] GenericArguments { get; set; }

        protected override void Initialize(MethodInfo memberInfo)
        {
            base.Initialize(memberInfo);
            if(!memberInfo.IsGenericMethod)
                return;
            
            this.IsGenericMethod = true;
            this.Signature = memberInfo.GetGenericMethodDefinition().ToString();
            this.GenericArguments = memberInfo.GetGenericArguments().Select(a => this.Factory.Create(a)).ToArray();
        }

        public override MethodInfo ToMemberInfo()
        {
            var method = base.ToMemberInfo();
            if (method == null)
                return null;

            if (this.IsGenericMethod && this.GenericArguments != null && this.GenericArguments.Length > 0)
            {
                var arguments = this.GenericArguments
                    .Select(a => a.ToType())
                    .Where(t => t != null).ToArray();       
                if(arguments.Length > 0)
                    method = method.MakeGenericMethod(arguments);
            }
            return method;
        }
    }
}