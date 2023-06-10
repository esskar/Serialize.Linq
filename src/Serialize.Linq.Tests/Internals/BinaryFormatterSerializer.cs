using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Internals
{
    internal class BinaryFormatterSerializer : SerializerBase, IBinarySerializer
    {
        private readonly BinaryFormatter _formatter;

        public BinaryFormatterSerializer()
        {
            _formatter = new BinaryFormatter();
        }

        public byte[] Serialize<T>(T obj) where T : Node
        {
            using (var ms = new MemoryStream())
            {
                Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] bytes) where T : Node
        {
            using (var ms = new MemoryStream(bytes))
                return Deserialize<T>(ms);
        }

        public void Serialize<T>(Stream stream, T obj) where T : Node
        {
            if(!ReferenceEquals(obj, null))
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                _formatter.Serialize(stream, obj);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
        }

        public T Deserialize<T>(Stream stream) where T : Node
        {
            if (stream.Length == 0)
                return default(T);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            return (T)_formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
        }
    }
}
