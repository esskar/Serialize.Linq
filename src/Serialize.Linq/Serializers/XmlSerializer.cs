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
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Serializers
{
    public class XmlSerializer : TextSerializer, IXmlSerializer
    {
#if !WINDOWS_PHONE && !NETSTANDARD && !WINDOWS_UWP
        protected override XmlObjectSerializer CreateXmlObjectSerializer(Type type)
        {
            return new DataContractSerializer(type, GetKnownTypes());
        }
#else
        private DataContractSerializer CreateDataContractSerializer(Type type)
        {
            return new DataContractSerializer(type, GetKnownTypes());
        }

        public override void Serialize<T>(Stream stream, T obj)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = CreateDataContractSerializer(typeof(T));
            serializer.WriteObject(stream, obj);
        }

        public override T Deserialize<T>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = CreateDataContractSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }
#endif
    }
}