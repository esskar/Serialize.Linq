using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    internal abstract class TextSerializer : DataSerializer, ITextSerializer
    {
        public string Serialize<T>(T obj)
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

        public T Deserialize<T>(string text)
        {
            using (var ms = new MemoryStream())
            {
                using(var writer = new StreamWriter(ms, Encoding.UTF8))
                    writer.Write(text);

                ms.Position = 0;
                return this.Deserialize<T>(ms);
            }
        }
    }
}