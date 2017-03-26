using System;
using System.Reflection;

namespace Serialize.Linq.Tests.Internals
{
    internal static class TypeExtensions
    {
        public static FieldInfo GetField(this Type type, string name)
        {
            return type.GetTypeInfo().GetField(name);
        }

        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetTypeInfo().GetMethod(name);
        }

        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetProperty(name);
        }
    }
}
