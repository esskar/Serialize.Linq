#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

#if !WINDOWS_PHONE && !NETSTANDARD && !WINDOWS_UWP
using System;
using System.Runtime.Serialization;
#endif
using System.IO;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Serializers
{
    public abstract class DataSerializer : SerializerBase, ISerializer
    {
#if !WINDOWS_PHONE && !NETSTANDARD && !WINDOWS_UWP
        public virtual void Serialize<TNode>(Stream stream, TNode obj) where TNode : Node
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = this.CreateXmlObjectSerializer(typeof(TNode));
            serializer.WriteObject(stream, obj);
        }

        public virtual TNode Deserialize<TNode>(Stream stream) where TNode : Node
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var serializer = this.CreateXmlObjectSerializer(typeof(TNode));
            return (TNode)serializer.ReadObject(stream);
        }

        protected abstract XmlObjectSerializer CreateXmlObjectSerializer(Type type);

#else

        public abstract void Serialize<TNode>(Stream stream, TNode obj) where TNode : Node;

        public abstract TNode Deserialize<TNode>(Stream stream) where TNode : Node;

#endif
    }
}
