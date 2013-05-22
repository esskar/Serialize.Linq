using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Tests.Internals
{
    internal class BinarayFormatterSerializer : IBinarySerializer
    {
        private readonly BinaryFormatter _formatter;

        public BinarayFormatterSerializer()
        {
            _formatter = new BinaryFormatter();
        }

        public byte[] Serialize<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                this.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
                return this.Deserialize<T>(ms);
        }

        public void Serialize<T>(Stream stream, T obj)
        {
            if(obj != null)
                _formatter.Serialize(stream, obj);
        }

        public T Deserialize<T>(Stream stream)
        {
            if (stream.Length == 0)
                return default(T);
            return (T)_formatter.Deserialize(stream);
        }        
    }
}
