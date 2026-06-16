using System;
using System.IO;
using Serialize.Linq.Nodes;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public abstract class DataSerializer : SerializerBase, ISerializer
    {
#if NETSTANDARD2_1 || NET6_0_OR_GREATER
        /// <summary>
        /// Gets or sets a custom <see cref="ISerializationSurrogateProvider"/> that is attached to the
        /// underlying data contract serializer. A surrogate provider lets callers substitute types during
        /// serialization and deserialization, for example to add support for types that the data contract
        /// serializer cannot handle on its own.
        /// </summary>
        /// <remarks>
        /// This property is not available on the .NET Framework / netstandard2.0 targets, which do not
        /// expose <see cref="ISerializationSurrogateProvider"/>.
        /// Note that the JSON serializer does not honor the surrogate provider on all runtimes
        /// (see https://github.com/dotnet/runtime/issues/100553); prefer the XML serializer when a
        /// surrogate provider is required.
        /// </remarks>
        public ISerializationSurrogateProvider SerializationSurrogateProvider { get; set; }
#endif

        public virtual void Serialize<T>(Stream stream, T obj) where T : Node
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (AutoDiscoverKnownTypes && obj != null)
                AddKnownTypes(Internals.KnownTypeDiscoverer.Discover(obj));

            var serializer = CreateSerializer(typeof(T));
            serializer.WriteObject(stream, obj);
        }

        public virtual T Deserialize<T>(Stream stream) where T : Node
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = CreateSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }

        private XmlObjectSerializer CreateSerializer(Type type)
        {
            var serializer = CreateXmlObjectSerializer(type);
#if NETSTANDARD2_1 || NET6_0_OR_GREATER
            // SetSerializationSurrogateProvider is only available on DataContractSerializer (XML).
            // DataContractJsonSerializer does not honor a surrogate provider (dotnet/runtime#100553).
            if (SerializationSurrogateProvider != null && serializer is DataContractSerializer dataContractSerializer)
                dataContractSerializer.SetSerializationSurrogateProvider(SerializationSurrogateProvider);
#endif
            return serializer;
        }

        protected abstract XmlObjectSerializer CreateXmlObjectSerializer(Type type);
    }
}
