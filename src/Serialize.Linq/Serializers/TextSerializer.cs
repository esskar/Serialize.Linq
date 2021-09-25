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
using System.Text;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public abstract class TextSerializer : GenericSerializerBase<string>, ITextSerializer
    {

        public override bool CanSerializeText => false;

        public override bool CanSerializeBinary => true;

        public override string Serialize<TNode>(TNode obj)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    this.Serialize(ms, obj);
                    ms.Position = 0;
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                        return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Error converting type: " + ex.Message, ex);
            }
        }

        public override TNode Deserialize<TNode>(string text)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms))
                {
                    writer.Write(text);
                    writer.Flush();
                    ms.Position = 0;
                    return this.Deserialize<TNode>(ms);
                }
            }
        }
    }
}