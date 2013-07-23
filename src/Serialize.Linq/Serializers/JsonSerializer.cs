#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public class JsonSerializer : TextSerializer, IJsonSerializer
    {
        protected override XmlObjectSerializer CreateXmlObjectSerializer(Type type)
        {
            return new DataContractJsonSerializer(type, this.GetKnownTypes());
        }
    }
}