#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

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
                this.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] bytes) where T : Node
        {
            using (var ms = new MemoryStream(bytes))
                return this.Deserialize<T>(ms);
        }

        public override void Serialize<T>(Stream stream, T obj)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var serializer = this.CreateXmlObjectSerializer(typeof(T));
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
            {
                serializer.WriteObject(writer, obj);
                writer.Flush();
            }
        }

        public override T Deserialize<T>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var serializer = this.CreateXmlObjectSerializer(typeof(T));
            using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
                return (T)serializer.ReadObject(reader);
        }

#if !WINDOWS_PHONE && !NETSTANDARD && !WINDOWS_UWP

        protected override XmlObjectSerializer CreateXmlObjectSerializer(Type type)
        {
            return new DataContractSerializer(type, this.GetKnownTypes());
        }        

#else

        private XmlObjectSerializer CreateXmlObjectSerializer(Type type)
        {
            return new DataContractSerializer(type, this.GetKnownTypes());
        }

#endif
    }
}
