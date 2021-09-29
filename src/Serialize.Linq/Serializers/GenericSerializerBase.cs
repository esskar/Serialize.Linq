using System;
using System.IO;
using System.Linq.Expressions;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Serializers
{
    public abstract class GenericSerializerBase<TSerialize> : DataSerializer, IGenericSerializer<TSerialize>
    {
        private readonly ExpressionConverter _converter = new ExpressionConverter();

        protected GenericSerializerBase(FactorySettings factorySettings = null)
        {
            FactorySettings = factorySettings;
        }

        public abstract bool CanSerializeBinary { get; }

        public abstract bool CanSerializeText { get; }

        public FactorySettings FactorySettings { get; }

        public TSerialize SerializeGeneric(Expression expression, FactorySettings factorySettings = null)
        {
            return Serialize(_converter.Convert(expression, factorySettings ?? FactorySettings));
        }

        public Expression DeserializeGeneric(TSerialize data, IExpressionContext context = null)
        {
            return Deserialize<ExpressionNode>(data)?.ToExpression(context ?? GetNewContext());
        }

        public override void Serialize(Stream stream, Expression expression, FactorySettings factorySettings = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            Serialize(stream, _converter.Convert(expression, factorySettings ?? FactorySettings));
        }

        public override Expression Deserialize(Stream stream, IExpressionContext context = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var node = Deserialize<ExpressionNode>(stream);
            return node?.ToExpression(context ?? GetNewContext());
        }

        public abstract TSerialize Serialize<TNode>(TNode obj) where TNode : Node;

        public abstract TNode Deserialize<TNode>(TSerialize data) where TNode : Node;

        private ExpressionContext GetNewContext()
        {
            return new ExpressionContext(FactorySettings != null && FactorySettings.AllowPrivateFieldAccess);
        }
    }
}
