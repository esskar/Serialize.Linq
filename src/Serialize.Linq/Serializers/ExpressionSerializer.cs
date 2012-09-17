using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Serializers
{
    public class ExpressionSerializer
    {
        private readonly ISerializer _serializer;
        private readonly ISerializerSettings _settings;

        public ExpressionSerializer(ISerializer serializer)
            : this(serializer, new SerializerSettings()) { }

        public ExpressionSerializer(ISerializer serializer, ISerializerSettings settings)
        {
            if(serializer == null)
                throw new ArgumentNullException("serializer");
            if(settings == null)
                throw new ArgumentNullException("settings");
            _serializer = serializer;
            _settings = settings;
        }

        public bool CanSerializeText
        {
            get { return _serializer is ITextSerializer; }
        }

        public bool CanSerializeBinary
        {
            get { return _serializer is IBinarySerializer; }
        }

        protected virtual INodeFactory CreateFactory(Expression expression, ISerializerSettings settings)
        {
            var lambda = expression as LambdaExpression;
            if(lambda != null)
                return new ComplexNodeFactory(settings, lambda.Parameters.Select(p => p.Type));
            return new NodeFactory(settings);
        }

        public void Serialize(Stream stream, Expression expression)
        {
            this.Serialize(stream, expression, _settings);
        }

        public void Serialize(Stream stream, Expression expression, ISerializerSettings settings)
        {
            if(stream == null)
                throw new ArgumentNullException("stream");
            if(settings == null)
                throw new ArgumentNullException("settings");

            var factory = this.CreateFactory(expression, settings);
            var expressionNode = factory.Create(expression);
            _serializer.Serialize(stream, expressionNode);
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
            return this.SerializeText(expression, _settings);
        }

        public string SerializeText(Expression expression, ISerializerSettings settings)
        {
            if(settings == null)
                throw new ArgumentNullException("settings");

            var factory = this.CreateFactory(expression, settings);
            var expressionNode = factory.Create(expression);
            return this.TextSerializer.Serialize(expressionNode);
        }

        public Expression DeserializeText(string text)
        {
            var node = this.TextSerializer.Deserialize<ExpressionNode>(text);
            return node != null ? node.ToExpression() : null;
        }

        public byte[] SerializeBinary(Expression expression)
        {
            return this.SerializeBinary(expression, _settings);
        }

        public byte[] SerializeBinary(Expression expression, ISerializerSettings settings)
        {
            if(settings == null)
                throw new ArgumentNullException("settings");

            var factory = this.CreateFactory(expression, settings);
            var expressionNode = factory.Create(expression);
            return this.BinarySerializer.Serialize(expressionNode);
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