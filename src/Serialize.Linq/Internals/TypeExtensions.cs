using System;
using System.Reflection;

namespace Serialize.Linq.Internals
{
    internal static class TypeExtensions
    {
#if NET40 || PORTABLE || SILVERLIGHT || WPSL
        // This allows us to use the new reflection API which separates Type and TypeInfo
        // while still supporting .NET 3.5 and 4.0. This class matches the API of the same
        // class in .NET 4.5+, and so is only needed on .NET Framework versions before that.
        //
        // Return the System.Type for now, we will probably need to create a TypeInfo class
        // which inherits from Type like .NET 4.5+ and implement the additional methods and
        // properties.
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
#endif
        public static bool IsArray(this Type type)
        {
            return type.IsArray;
        }

        public static bool IsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        public static bool IsSubclassOf(this Type type, Type c)
        {
            return type.GetTypeInfo().IsSubclassOf(c);
        }

        public static bool IsCustomAttributeDefined(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().IsDefined(attributeType, inherit);
        }

        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        public static bool IsInterface(this Type type)
        {
            return type.GetTypeInfo().IsInterface;
        }

        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        public static Type GetBaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        public static TypeAttributes GetTypeAttributes(this Type type)
        {
            return type.GetTypeInfo().Attributes;
        }
    }
}