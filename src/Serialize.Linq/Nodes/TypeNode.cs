using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "T")]
#endif
    #endregion
    public class TypeNode : Node
    {
        private static readonly ConcurrentDictionary<string, Type> __typeCache;

        static TypeNode()
        {
            __typeCache = new ConcurrentDictionary<string, Type>();
        }

        public TypeNode() { }

        public TypeNode(INodeFactory factory, Type type)
            : base(factory)
        {
            this.Initialize(type);
        }

        private void Initialize(Type type)
        {
            if (type == null)
                return;

            if (type.IsGenericType)
            {
                this.GenericArguments = type.GetGenericArguments().Select(t => new TypeNode(this.Factory, t)).ToArray();
                this.Name = type.GetGenericTypeDefinition().FullName;
                
            }
            else
            {
                this.Name = type.FullName;
            }            
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "N")]        
#endif
        #endregion
        public string Name { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "G")]        
#endif
        #endregion
        public TypeNode[] GenericArguments { get; set; }

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
                        if (type != null)
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

            if (this.GenericArguments != null)            
                type = type.MakeGenericType(this.GenericArguments.Select(t => t.ToType()).ToArray());
            
            return type;
        }
    }
}