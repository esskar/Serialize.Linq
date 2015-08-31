#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Serializers
{
    public class ExpressionSerializer : ExpressionConverter
    {
        private readonly ISerializer _serializer;
        private readonly FactorySettings _factorySettings;

        public ExpressionSerializer(ISerializer serializer, FactorySettings factorySettings = null)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");
            _serializer = serializer;
            _factorySettings = factorySettings;
        }

        public bool AutoAddKnownTypesAsArrayTypes
        {
            get { return _serializer.AutoAddKnownTypesAsArrayTypes; }
            set { _serializer.AutoAddKnownTypesAsArrayTypes = value; }
        }

        public bool AutoAddKnownTypesAsListTypes
        {
            get { return _serializer.AutoAddKnownTypesAsListTypes; }
            set { _serializer.AutoAddKnownTypesAsListTypes = value; }
        }

        public bool CanSerializeText
        {
            get { return _serializer is ITextSerializer; }
        }

        public bool CanSerializeBinary
        {
            get { return _serializer is IBinarySerializer; }
        }

        public void AddKnownType(Type type)
        {
            _serializer.AddKnownType(type);
        }

        public void AddKnownTypes(IEnumerable<Type> types)
        {
            _serializer.AddKnownTypes(types);
        }

        public void Serialize(Stream stream, Expression expression, FactorySettings factorySettings = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            _serializer.Serialize(stream, this.Convert(expression, factorySettings ?? _factorySettings));
        }

        public Expression Deserialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var node = _serializer.Deserialize<ExpressionNode>(stream);
            return node != null ? node.ToExpression() : null;
        }

        public string SerializeText(Expression expression, FactorySettings factorySettings = null)
        {
            return this.TextSerializer.Serialize(this.Convert(expression, factorySettings ?? _factorySettings));
        }

        public Expression DeserializeText(string text)
        {
            var node = this.TextSerializer.Deserialize<ExpressionNode>(text);
            return node == null ? null : node.ToExpression();
        }

        public Expression DeserializeText(string text, ExpressionContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var node = this.TextSerializer.Deserialize<ExpressionNode>(text);
            return node == null ? null : node.ToExpression(context);
        }

        public byte[] SerializeBinary(Expression expression, FactorySettings factorySettings = null)
        {
            return this.BinarySerializer.Serialize(this.Convert(expression, factorySettings ?? _factorySettings));
        }

        public Expression DeserializeBinary(byte[] bytes)
        {
            var node = this.BinarySerializer.Deserialize<ExpressionNode>(bytes);
            return node == null ? null : node.ToExpression();
        }

        public Expression DeserializeBinary(byte[] bytes, ExpressionContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var node = this.BinarySerializer.Deserialize<ExpressionNode>(bytes);
            return node == null ? null : node.ToExpression(context);
        }

        private ITextSerializer TextSerializer
        {
            get
            {
                var textSerializer = _serializer as ITextSerializer;
                if (textSerializer == null)
                    throw new InvalidOperationException("Unable to serialize text.");
                return textSerializer;
            }
        }

        private IBinarySerializer BinarySerializer
        {
            get
            {
                var binarySerializer = _serializer as IBinarySerializer;
                if (binarySerializer == null)
                    throw new InvalidOperationException("Unable to serialize binary.");
                return binarySerializer;
            }
        }
    }
}