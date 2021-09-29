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
using System.Runtime.Serialization.Formatters.Binary;
using Serialize.Linq.Extensions;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Internals
{
    internal class BinaryFormatterSerializer : SerializerBase, IBinarySerializer
    {
        private readonly ExpressionConverter _converter = new ExpressionConverter();

        private readonly BinaryFormatter _formatter;

        public bool CanSerializeText
        {
            get
            {
                return false;
            }
        }

        public bool CanSerializeBinary
        {
            get
            {
                return true;
            }
        }

        public BinaryFormatterSerializer()
        {
            _formatter = new BinaryFormatter();
        }

        public byte[] Serialize<T>(T obj) where T : Node
        {
            using (var ms = new MemoryStream())
            {
                this.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] bytes) where T : Node
        {
            using (var ms = new MemoryStream(bytes))
                return this.Deserialize<T>(ms);
        }

        public void Serialize<T>(Stream stream, T obj) where T : Node
        {
            if (!ReferenceEquals(obj, null))
#pragma warning disable SYSLIB0011 // type or element is obsolete
                _formatter.Serialize(stream, obj);
#pragma warning restore SYSLIB0011 // type or element is obsolete
        }

        public T Deserialize<T>(Stream stream) where T : Node
        {
            if (stream.Length == 0)
                return default(T);
#pragma warning disable SYSLIB0011 // type or element is obsolete
            return (T)_formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // type or element is obsolete
        }

        public byte[] SerializeGeneric(Expression expression, FactorySettings settings = null)
        {
            return Serialize(expression.ToExpressionNode(settings));
        }

        public Expression DeserializeGeneric(byte[] data, IExpressionContext context = null)
        {
            return Deserialize<ExpressionNode>(data)?.ToExpression(context ?? new ExpressionContext(false));
        }

        public void Serialize(Stream stream, Expression expression, FactorySettings factorySettings = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            Serialize(stream, _converter.Convert(expression, factorySettings));
        }

        public Expression Deserialize(Stream stream, IExpressionContext context = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var node = Deserialize<ExpressionNode>(stream);
            return node?.ToExpression(context);
        }
    }
}
