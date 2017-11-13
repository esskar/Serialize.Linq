﻿#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

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
#if !SILVERLIGHT && !NETCOREAPP1_1 && !NETSTANDARD1_6 && !NETCOREAPP2_0 && !NETFX_CORE && !WINDOWS_UWP
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
            else if (this.Factory != null)
                flags = this.Factory.GetBindingFlags();
            return flags == null ? type.GetMembers() : type.GetMembers(flags.Value);
        }
    }
}