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
    [DataContract(Name = "FI")]
#endif
#if !(SILVERLIGHT || DNXCORE50 || DOTNET5_4)
    [Serializable]
#endif
    #endregion
    public class FieldInfoNode : MemberNode<FieldInfo>
    {
        public FieldInfoNode() { }

        public FieldInfoNode(INodeFactory factory, FieldInfo memberInfo)
            : base(factory, memberInfo) { }

        protected override IEnumerable<FieldInfo> GetMemberInfosForType(ExpressionContext context, Type type)
        {
            return type.GetFields();
        }
    }
}