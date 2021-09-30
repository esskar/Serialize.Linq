#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public abstract class TextSerializer : GenericSerializerBase<string>, ITextTypeSerializer
    {
        protected TextSerializer()
            : base() { }

        protected TextSerializer(FactorySettings factorySettings)
            : base(factorySettings) { }

        public bool CanSerializeText => true;

        public bool CanSerializeBinary => false;

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

        [Obsolete("This function is just for compatibility. Please use SerializeGeneric(Expression, FactorySettings) instead.", false)]
        public string SerializeText(Expression expression, FactorySettings factorySettings = null)
        {
            return  SerializeGeneric(expression,factorySettings);
        }

        [Obsolete("This function is just for compatibility. Please use DeserializeGeneric(byte[], IExpressionContext) instead.", false)]
        public Expression DeserializeText(string data, IExpressionContext context = null)
        {
            return DeserializeGeneric(data, context);
        }

        [Obsolete("This function is just for compatibility. It throws a NotImplementedException.", true)]
        public byte[] SerializeBinary(Expression expression, FactorySettings factorySettings = null)
        {
            throw new NotImplementedException();
        }

        [Obsolete("This function is just for compatibility. It throws a NotImplementedException.", true)]
        public Expression DeserializeBinary(byte[] bytes, IExpressionContext context = null)
        {
            throw new NotImplementedException();
        }
    }
}