#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

#if !DNXCORE50
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Internals
{
    internal class BinaryFormatterSerializer : SerializerBase, IBinarySerializer
    {
        private readonly BinaryFormatter _formatter;

        public BinaryFormatterSerializer()
        {
            _formatter = new BinaryFormatter();
        }

        public byte[] Serialize<T>(T obj) where T : Node
        {
            using (var ms = new MemoryStream())
            {
                this.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] bytes) where T : Node
        {
            using (var ms = new MemoryStream(bytes))
                return this.Deserialize<T>(ms);
        }

        public void Serialize<T>(Stream stream, T obj) where T : Node
        {
            if (!ReferenceEquals(obj, null))
                _formatter.Serialize(stream, obj);
        }

        public T Deserialize<T>(Stream stream) where T : Node
        {
            if (stream.Length == 0)
                return default(T);
            return (T)_formatter.Deserialize(stream);
        }
    }
}
#endif