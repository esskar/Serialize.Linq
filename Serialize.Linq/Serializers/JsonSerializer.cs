using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public class JsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T obj)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    serializer.WriteObject(ms, obj);
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

                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}