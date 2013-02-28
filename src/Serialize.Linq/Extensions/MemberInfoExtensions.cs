using System;
using System.Reflection;

namespace Serialize.Linq.Extensions
{
    public static class MemberInfoExtensions
    {
        public static Type GetReturnType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;

                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;

                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;

                default:
                    throw new NotSupportedException("Unable to get return type of member of type " + member.MemberType);
            }
        }
    }
}