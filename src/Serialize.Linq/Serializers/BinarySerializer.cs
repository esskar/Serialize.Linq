using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Serializers
{
    public class BinarySerializer : DataSerializer, IBinarySerializer
    {
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

        public override void Serialize<T>(Stream stream, T obj)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = CreateXmlObjectSerializer(typeof(T));
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
            {
                serializer.WriteObject(writer, obj);
                writer.Flush();
            }
        }

        public override T Deserialize<T>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = CreateXmlObjectSerializer(typeof(T));
            using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
                return (T)serializer.ReadObject(reader);
        }

#if !NETSTANDARD && !WINDOWS_UWP

        protected override XmlObjectSerializer CreateXmlObjectSerializer(Type type)
        {
            return new DataContractSerializer(type, GetKnownTypes());
        }        

#else

        private XmlObjectSerializer CreateXmlObjectSerializer(Type type)
        {
            return new DataContractSerializer(type, GetKnownTypes());
        }

#endif
    }
}
