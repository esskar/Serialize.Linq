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
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public class XmlSerializer : TextSerializer, IXmlSerializer
    {
        public XmlSerializer()
            : base() { }

        public XmlSerializer(FactorySettings factorySettings)
            : base(factorySettings) { }

#if !WINDOWS_PHONE && !NETSTANDARD && !WINDOWS_UWP
        protected override XmlObjectSerializer CreateXmlObjectSerializer(Type type)
        {
            return new DataContractSerializer(type, this.GetKnownTypes());
        }
#else
        private DataContractSerializer CreateDataContractSerializer(Type type)
        {
            return new DataContractSerializer(type, this.GetKnownTypes());
        }

        public override void Serialize<TNode>(Stream stream, TNode obj)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = this.CreateDataContractSerializer(typeof(TNode));
            serializer.WriteObject(stream, obj);
        }

        public override TNode Deserialize<TNode>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = this.CreateDataContractSerializer(typeof(TNode));
            return (TNode)serializer.ReadObject(stream);
        }
#endif
    }
}