using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class TypeNode : Node
    {
        private static readonly ConcurrentDictionary<string, Type> __typeCache;

        static TypeNode()
        {
            __typeCache = new ConcurrentDictionary<string, Type>();
        }

        public TypeNode(INodeFactory factory, Type type)
            : base(factory)
        {
            this.Name = BuildName(type);            
        }

        private static string BuildName(Type type)
        {
            if (type == null)
                return null;

            var name = type.FullName;
            if (type.IsGenericType && name != null)
            {
                foreach (var paramType in type.GetGenericArguments().Distinct())
                {
                    var assemblyQualifiedName = paramType.AssemblyQualifiedName;
                    if (string.IsNullOrWhiteSpace(assemblyQualifiedName)) continue;

                    name = name.Replace(assemblyQualifiedName, BuildName(paramType));
                }
            }

            return name;
        }

        [DataMember]        
        public string Name { get; set; }

        private static Type ResolveType(string typeName)
        {            
            return __typeCache.GetOrAdd(typeName, n =>
            {
                var type = Type.GetType(n);
                if (type == null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        type = assembly.GetType(n);
                        if(type != null)
                            break;
                    }
                    
                }
                return type;
            });
        }

        public Type ToType()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
                return null;

            var type = ResolveType(this.Name);
            if (type == null)
                throw new SerializationException(string.Format("Failed to serialize '{0}' to a type object.", this.Name));
            return type;
        }
    }    
}