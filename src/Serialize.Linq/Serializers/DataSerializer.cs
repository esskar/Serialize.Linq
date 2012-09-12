using System;
using System.IO;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    internal abstract class DataSerializer : ISerializer
    {
        protected abstract XmlObjectSerializer CreateXmlObjectSerializer(Type type);

        public virtual void Serialize<T>(Stream stream, T obj)
        {
            if(stream == null)
                throw new ArgumentNullException("stream");

            var serializer = this.CreateXmlObjectSerializer(typeof(T));
            serializer.WriteObject(stream, obj);
        }

        public virtual T Deserialize<T>(Stream stream)
        {
            if(stream == null)
                throw new ArgumentNullException("stream");

            var serializer = this.CreateXmlObjectSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }        
    }
}
