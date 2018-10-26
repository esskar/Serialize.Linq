#region Copyright
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
#if SERIALIZE_LINQ_BORKED_VERION
    #if SERIALIZE_LINQ_WITH_LONG_DATA_NAMES
        [DataContract]
    #else
        [DataContract(Name = "PI")]
    #endif
#else
    #if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataContract]
#else
        [DataContract(Name = "PI")]
#endif
#endif
#if !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class PropertyInfoNode : MemberNode<PropertyInfo>
    {
        public PropertyInfoNode() { }

        public PropertyInfoNode(INodeFactory factory, PropertyInfo memberInfo) 
            : base(factory, memberInfo) { }

        protected override IEnumerable<PropertyInfo> GetMemberInfosForType(IExpressionContext context, Type type)
        {
            return type.GetProperties();
        }
    }
}