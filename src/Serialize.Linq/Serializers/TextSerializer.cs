using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Serializers
{
    public abstract class TextSerializer : DataSerializer, ITextSerializer
    {
        public string Serialize<T>(T obj) where T : Node
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    Serialize(ms, obj);

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

        public T Deserialize<T>(string text) where T : Node
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms))
                {
                    writer.Write(text);
                    writer.Flush();

                    ms.Position = 0;
                    return Deserialize<T>(ms);
                }
            }
        }
    }
}