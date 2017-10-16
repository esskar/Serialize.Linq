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
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsClass;
        }

        public static bool IsCustomAttributeDefined(this Type type, Type attributeType, bool inherit)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.GetCustomAttribute(attributeType, inherit) != null;
        }

        public static bool IsEnum(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsEnum;
        }

        public static bool IsInterface(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsInterface;
        }

        public static bool IsGenericType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsGenericType;
        }

        public static bool IsValueType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsValueType;
        }

        public static Type GetBaseType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.BaseType;
        }

        public static FieldInfo GetField(this Type type, string fieldName)
        {
            return System.Reflection.TypeExtensions.GetField(type, fieldName);
        }

        public static PropertyInfo GetProperty(this Type type, string propertyName)
        {
            return System.Reflection.TypeExtensions.GetProperty(type, propertyName);
        }

        public static MethodInfo GetMethod(this Type type, string methodName)
        {
            return System.Reflection.TypeExtensions.GetMethod(type, methodName);
        }

        public static TypeAttributes GetTypeAttributes(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.Attributes;
        }

        public static bool IsSubclassOf(this Type type, Type c)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsSubclassOf(c);
        }
    }
}
