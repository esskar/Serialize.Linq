using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Serialize.Linq.Internals
{
    internal static class SerializationHelper
    {
        public static string SerializeType(Type type)
        {
            return type != null ? type.AssemblyQualifiedName : null;
        }

        public static Type DeserializeType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return null;
            var type = Type.GetType(typeName);
            if (type == null)
                throw new SerializationException(string.Format("Failed to serialize '{0}' to a type object.", typeName));
            return type;
        }

        public static string SerializeMethod(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                return null;

            var sb = new StringBuilder();
            sb.Append(SerializeType(methodInfo.DeclaringType));
            if (sb.Length == 0)
                throw new SerializationException(string.Format("Failed to serialize declaring type '{0}' of method '{1}", methodInfo.DeclaringType, methodInfo));            
            
            sb.AppendLine();
            if (!methodInfo.IsGenericMethod)
            {
                sb.Append(methodInfo.ToString());
                return sb.ToString();
            }
            
            sb.Append(methodInfo.GetGenericMethodDefinition());
            var argumentTypes = methodInfo.GetGenericArguments(); 
            if(argumentTypes.Length > 0)
            {
                sb.AppendLine();
                foreach (var type in argumentTypes)
                    sb.AppendLine(SerializeType(type));
            }
            return sb.ToString();
        }

        public static MethodInfo DeserializeMethod(string methodInfo)
        {
            if (string.IsNullOrWhiteSpace(methodInfo))
                return null;

            var lines = GetLines(methodInfo);
            if (lines.Length < 2)
                throw new FormatException("The value to deserialize has an invalid format.");

            var type = DeserializeType(lines[0]);
            if (type == null)
                throw new TypeLoadException(string.Format("Failed to deserialize type '{0}'", lines[0]));

            var method = type.GetMethods().First(m => m.ToString() == lines[1]);
            if (method.IsGenericMethod && lines.Length > 2)
            {
                var arguments = lines.Skip(2)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(DeserializeType)
                    .Where(t => t != null).ToArray();       
                if(arguments.Length > 0)
                    method = method.MakeGenericMethod(arguments);
            }
            return method;            
        }

        public static string SerializeMember(MemberInfo memberInfo)
        {
            if (memberInfo == null)
                return null;

            var sb = new StringBuilder();
            sb.Append(SerializeType(memberInfo.DeclaringType));
            if (sb.Length == 0)
                throw new SerializationException(string.Format("Failed to serialize declaring type '{0}' of member '{1}", memberInfo.DeclaringType, memberInfo));

            sb.AppendLine();
            sb.Append(memberInfo);

            return sb.ToString();
        }

        public static MemberInfo DeserializeMember(string memberInfo)
        {
            if (string.IsNullOrWhiteSpace(memberInfo))
                return null;

            var lines = GetLines(memberInfo);
            if (lines.Length < 2)
                throw new FormatException("The value to deserialize has an invalid format.");
            
            var type = DeserializeType(lines[0]);
            if (type == null)
                throw new TypeLoadException(string.Format("Failed to deserialize type '{0}'", lines[0]));
            return type.GetMembers().First(m => m.ToString() == lines[1]);
        }

        public static string SerializeConstructor(ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null)
                return null;

            var sb = new StringBuilder();
            sb.Append(SerializeType(constructorInfo.DeclaringType));
            if (sb.Length == 0)
                throw new SerializationException(string.Format("Failed to serialize declaring type '{0}' of member '{1}", constructorInfo.DeclaringType, constructorInfo));

            sb.AppendLine();
            sb.Append(constructorInfo);

            return sb.ToString();
        }

        public static ConstructorInfo DeserializeConstructor(string constructorInfo)
        {
            if (string.IsNullOrWhiteSpace(constructorInfo))
                return null;

            var lines = GetLines(constructorInfo);
            if (lines.Length < 2)
                throw new FormatException("The value to deserialize has an invalid format.");
            
            var type = DeserializeType(lines[0]);
            if (type == null)
                throw new TypeLoadException(string.Format("Failed to deserialize type '{0}'", lines[0]));
            return type.GetConstructors().First(m => m.ToString() == lines[1]);
        }

        private static string[] GetLines(string value)
        {
            if(value == null)
                return new string[0];
            var pattern = value.Contains(Environment.NewLine) ? Environment.NewLine : "\n";
            return value.Split(new [] { pattern }, StringSplitOptions.None);
        }
    }
}
