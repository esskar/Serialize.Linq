using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "MI")]
#endif
#if !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class MemberInfoNode : MemberNode<MemberInfo>
    {
        public MemberInfoNode() { }

        public MemberInfoNode(INodeFactory factory, MemberInfo memberInfo)
            : base(factory, memberInfo) { }

        protected override IEnumerable<MemberInfo> GetMemberInfosForType(IExpressionContext context, Type type)
        {
            BindingFlags? flags = null;
            if (context != null)
                flags = context.GetBindingFlags();
            else if (Factory != null)
                flags = Factory.GetBindingFlags();
            return flags == null ? type.GetMembers() : type.GetMembers(flags.Value);
        }
    }
}