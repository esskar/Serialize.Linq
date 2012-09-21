using System;
using System.IO;
using System.Linq.Expressions;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Serializers
{
    public class ExpressionSerializer : ExpressionConverter
    {
        private readonly ISerializer _serializer;
        
        public ExpressionSerializer(ISerializer serializer)
        {
            if(serializer == null)
                throw new ArgumentNullException("serializer");
            _serializer = serializer;            
        }

        public bool CanSerializeText
        {
            get { return _serializer is ITextSerializer; }
        }

        public bool CanSerializeBinary
        {
            get { return _serializer is IBinarySerializer; }
        }

        public void Serialize(Stream stream, Expression expression)
        {
            if(stream == null)
                throw new ArgumentNullException("stream");            
            _serializer.Serialize(stream, this.Convert(expression));
        }

        public Expression Deserialize(Stream stream)
        {
            if(stream == null)
                throw new ArgumentNullException("stream");

            var node = _serializer.Deserialize<ExpressionNode>(stream);
            return node != null ? node.ToExpression() : null;
        }        

        public string SerializeText(Expression expression)
        {
            return this.TextSerializer.Serialize(this.Convert(expression));
        }

        public Expression DeserializeText(string text)
        {
            var node = this.TextSerializer.Deserialize<ExpressionNode>(text);
            return node == null ? null : node.ToExpression();
        }

        public byte[] SerializeBinary(Expression expression)
        {
            return this.BinarySerializer.Serialize(this.Convert(expression));
        }

        public Expression DeserializeBinary(byte[] bytes)
        {
            var node = this.BinarySerializer.Deserialize<ExpressionNode>(bytes);
            return node != null ? node.ToExpression() : null;
        }

        private ITextSerializer TextSerializer
        {
            get 
            {
                var textSerializer = _serializer as ITextSerializer;
                if(textSerializer == null)
                    throw new InvalidOperationException("Unable to serialize text.");
                return textSerializer;
            }
        }

        private IBinarySerializer BinarySerializer
        {
            get 
            {
                var binarySerializer = _serializer as IBinarySerializer;
                if(binarySerializer == null)
                    throw new InvalidOperationException("Unable to serialize binary.");
                return binarySerializer;
            }
        }
    }
}