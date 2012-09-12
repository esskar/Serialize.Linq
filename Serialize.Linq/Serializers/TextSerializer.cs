using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Serialize.Linq.Serializers
{
    public abstract class TextSerializer
    {
        protected abstract XmlObjectSerializer CreateXmlObjectSerializer(Type type);

        public string Serialize<T>(T obj)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    var serializer = this.CreateXmlObjectSerializer(typeof(T));
                    serializer.WriteObject(ms, obj);

                    ms.Position = 0;
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                        return reader.ReadToEnd();                    
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("JsonDataContractSerializer: Error converting type: " + ex.Message, ex);
            }
        }

        public T Deserialize<T>(string data)
        {
            using (var ms = new MemoryStream())
            {
                using(var writer = new StreamWriter(ms, Encoding.UTF8))
                    writer.Write(data);

                var serializer = this.CreateXmlObjectSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
