using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Internals
{
    internal class BinarayFormatterSerializer : SerializerBase, IBinarySerializer
    {
        private readonly BinaryFormatter _formatter;

        public BinarayFormatterSerializer()
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
                _formatter.Serialize(stream, obj);
        }

        public T Deserialize<T>(Stream stream) where T : Node
        {
            if (stream.Length == 0)
                return default(T);
            return (T)_formatter.Deserialize(stream);
        }        
    }
}
