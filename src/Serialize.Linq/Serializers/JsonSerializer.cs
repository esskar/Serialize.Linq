﻿#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.IO;
#if !WINDOWS_PHONE && !NETCOREAPP1_1 && !NETSTANDARD1_6 && !NETCOREAPP2_0
using System.Runtime.Serialization;
#endif
using System.Runtime.Serialization.Json;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public class JsonSerializer : TextSerializer, IJsonSerializer
    {
#if !WINDOWS_PHONE && !NETCOREAPP1_1 && !NETSTANDARD1_6 && !NETCOREAPP2_0 && !NETFX_CORE && !WINDOWS_UWP 
        protected override XmlObjectSerializer CreateXmlObjectSerializer(Type type)
        {
            return new DataContractJsonSerializer(type, this.GetKnownTypes());
        }
#else
        private DataContractJsonSerializer CreateDataContractJsonSerializer(Type type)
        {
            return new DataContractJsonSerializer(type, this.GetKnownTypes());
        }

        public override void Serialize<T>(Stream stream, T obj)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var serializer = this.CreateDataContractJsonSerializer(typeof(T));
            serializer.WriteObject(stream, obj);
        }

        public override T Deserialize<T>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var serializer = this.CreateDataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }
#endif
    }
}