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
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public class BinarySerializer : GenericSerializerBase<byte[]>, IBinarySerializer
    {
        public BinarySerializer()
            : base() { }

        public BinarySerializer(FactorySettings factorySettings)
            : base(factorySettings) { }

        public override bool CanSerializeText => false;

        public override bool CanSerializeBinary => true;

        public override byte[] Serialize<TNode>(TNode obj)
        {
            using (var ms = new MemoryStream())
            {
                this.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public override TNode Deserialize<TNode>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
                return this.Deserialize<TNode>(ms);
        }

        public override void Serialize<TNode>(Stream stream, TNode obj)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = this.CreateXmlObjectSerializer(typeof(TNode));
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
            {
                serializer.WriteObject(writer, obj);
                writer.Flush();
            }
        }

        public override TNode Deserialize<TNode>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = this.CreateXmlObjectSerializer(typeof(TNode));
            using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
                return (TNode)serializer.ReadObject(reader);
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
