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
    [Serializable]
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

            IsGenericMethod = true;
            Signature = memberInfo.GetGenericMethodDefinition().ToString();
            GenericArguments = memberInfo.GetGenericArguments().Select(a => Factory.Create(a)).ToArray();
        }

        public override MethodInfo ToMemberInfo(IExpressionContext context)
        {
            var method = base.ToMemberInfo(context);
            if (method == null)
                return null;

            if (IsGenericMethod && GenericArguments != null && GenericArguments.Length > 0)
            {
                var arguments = GenericArguments
                    .Select(a => a.ToType(context))
                    .Where(t => t != null).ToArray();
                if (arguments.Length > 0)
                    method = method.MakeGenericMethod(arguments);
            }
            return method;
        }
    }
}