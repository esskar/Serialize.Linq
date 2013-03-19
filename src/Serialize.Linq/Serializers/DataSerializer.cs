using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public abstract class DataSerializer : ISerializer
    {
        protected abstract XmlObjectSerializer CreateXmlObjectSerializer(Type type);

        private static readonly Type[] _knownTypes = new [] { 
            typeof(bool),
            typeof(decimal), typeof(double),
            typeof(float),
            typeof(int), typeof(uint),
            typeof(short), typeof(ushort),
            typeof(long), typeof(ulong),
            typeof(string),
            typeof(DateTime), typeof(TimeSpan), typeof(Guid),             
        };

        protected virtual IEnumerable<Type> GetKnownTypes()
        {
            foreach (var knownType in _knownTypes)
            {
                yield return knownType;
                yield return knownType.MakeArrayType();
                if (!knownType.IsClass)
                    yield return typeof(Nullable<>).MakeGenericType(knownType);
            }
        }

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
