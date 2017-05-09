using System;
using System.Reflection;

namespace Serialize.Linq.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsArray(this Type type)
        {
            return type.IsArray;
        }

        public static bool IsClass(this Type type)
        {
            return type.IsClass;
        }

        public static bool IsCustomAttributeDefined(this Type type, Type attributeType, bool inherit)
        {
            return Attribute.IsDefined(type, attributeType, inherit);
        }

        public static bool IsEnum(this Type type)
        {
            return type.IsEnum;
        }

        public static bool IsInterface(this Type type)
        {
            return type.IsInterface;
        }

        public static bool IsGenericType(this Type type)
        {
            return type.IsGenericType;
        }

        public static bool IsValueType(this Type type)
        {
            return type.IsValueType;
        }

        public static Type GetBaseType(this Type type)
        {
            return type.BaseType;
        }

        public static TypeAttributes GetTypeAttributes(this Type type)
        {
            return type.Attributes;
        }
    }
}
