#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.IO;
#if !WINDOWS_PHONE
using System.Runtime.Serialization;
#endif
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public abstract class DataSerializer : ISerializer
    {        
        private static readonly Type[] _knownTypes = new[] { 
            typeof(bool),
            typeof(decimal), typeof(double),
            typeof(float),
            typeof(int), typeof(uint),
            typeof(short), typeof(ushort),
            typeof(long), typeof(ulong),
            typeof(string),
            typeof(DateTime), typeof(TimeSpan), typeof(Guid)
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

#if !WINDOWS_PHONE

        public virtual void Serialize<T>(Stream stream, T obj)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var serializer = this.CreateXmlObjectSerializer(typeof(T));
            serializer.WriteObject(stream, obj);
        }

        public virtual T Deserialize<T>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var serializer = this.CreateXmlObjectSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }

        protected abstract XmlObjectSerializer CreateXmlObjectSerializer(Type type);

#else

        public abstract void Serialize<T>(Stream stream, T obj);

        public abstract T Deserialize<T>(Stream stream);

#endif
    }
}
