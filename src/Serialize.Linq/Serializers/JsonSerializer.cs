using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    internal class JsonSerializer : TextSerializer, IJsonSerializer
    {
        protected override XmlObjectSerializer CreateXmlObjectSerializer(Type type)
        {
            return new DataContractJsonSerializer(type, this.GetKnownTypes());
        }
    }
}