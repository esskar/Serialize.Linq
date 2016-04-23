#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Reflection;

namespace Serialize.Linq.Extensions
{
    /// <summary>
    /// MemberInfo extensions methods.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets the return type of a member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">Unable to get return type of member of type {member.MemberType}</exception>
        public static Type GetReturnType(this MemberInfo member)
        {
            // https://github.com/dotnet/corefx/issues/4670
            PropertyInfo property = member as PropertyInfo;
            if (property != null)
                return property.PropertyType;

            MethodInfo method = member as MethodInfo;
            if (method != null)
                return method.ReturnType;

            FieldInfo field = member as FieldInfo;
            if (field != null)
                return field.FieldType;

            throw new NotSupportedException("Unable to get return type of MemberInfo of type " + member);
        }
    }
}