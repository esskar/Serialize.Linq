using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;

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

        protected virtual INodeFactory CreateFactory(Expression expression, ISerializerSettings settings)
        {
            var lambda = expression as LambdaExpression;
            if(lambda != null)
                return  new ComplexNodeFactory(settings, lambda.Parameters.Select(p => p.Type));
            return new NodeFactory(settings);
        }

        public void Serialize<TExpression>(Stream stream, TExpression expression) where TExpression : Expression
        {
            this.Serialize(stream, expression, _settings);
        }

        public void Serialize<TExpression>(Stream stream, TExpression expression, ISerializerSettings settings) where TExpression: Expression
        {
            if(settings == null)
                throw new ArgumentNullException("settings");

            var factory = this.CreateFactory(expression, settings);
            var expressionNode = factory.Create(expression);
            _serializer.Serialize(stream, expressionNode);
        }

        public TExpression Deserialize<TExpression>(Stream stream) where TExpression : Expression
        {
            return null;
        }
    }
}