#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "MIN")]
#endif
#if !SILVERLIGHT && !NETCOREAPP1_1
    [Serializable]
#endif
    #endregion
    public class MethodInfoNode : MemberNode<MethodInfo>
    {
        public MethodInfoNode() { }

        public MethodInfoNode(INodeFactory factory, MethodInfo memberInfo)
            : base(factory, memberInfo) { }

        protected override IEnumerable<MethodInfo> GetMemberInfosForType(IExpressionContext context, Type type)
        {
            return type.GetMethods();
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "I")]
#endif
        #endregion
        public bool IsGenericMethod { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "G")]
#endif
        #endregion
        public TypeNode[] GenericArguments { get; set; }

        protected override void Initialize(MethodInfo memberInfo)
        {
            base.Initialize(memberInfo);
            if (!memberInfo.IsGenericMethod)
                return;

            this.IsGenericMethod = true;
            this.Signature = memberInfo.GetGenericMethodDefinition().ToString();
            this.GenericArguments = memberInfo.GetGenericArguments().Select(a => this.Factory.Create(a)).ToArray();
        }

        public override MethodInfo ToMemberInfo(IExpressionContext context)
        {
            var method = base.ToMemberInfo(context);
            if (method == null)
                return null;

            if (this.IsGenericMethod && this.GenericArguments != null && this.GenericArguments.Length > 0)
            {
                var arguments = this.GenericArguments
                    .Select(a => a.ToType(context))
                    .Where(t => t != null).ToArray();
                if (arguments.Length > 0)
                    method = method.MakeGenericMethod(arguments);
            }
            return method;
        }
    }
}