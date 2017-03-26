using System;
using System.Reflection;

namespace Serialize.Linq.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsArray(this Type type)
        {
            return type.GetTypeInfo().IsArray;
        }

        public static bool IsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;
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

        public static bool IsSubclassOf(this Type type, Type t)
        {
            return type.GetTypeInfo().IsSubclassOf(t);
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        public static Type GetBaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        public static bool IsCustomAttributeDefined(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttribute(attributeType, inherit) != null;
        }

        public static TypeAttributes GetTypeAttributes(this Type type)
        {
            return type.GetTypeInfo().Attributes;
        }
    }
}
