#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Serializers;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests
{
    [TestClass]
    public class ExpressionSerializerTestsGeneric
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CanSerializeTextTest()
        {
            foreach (var textSerializer in CreateTextSerializers())
            {
                var serializer = textSerializer;
                Assert.IsTrue(serializer.CanSerializeText, "'{0}' was expected to serialize text.", textSerializer.GetType());
            }

            foreach (var binSerializer in CreateBinarySerializers())
            {
                var serializer = binSerializer;
                Assert.IsFalse(serializer.CanSerializeText, "'{0}' was not expected to serialize text.", serializer.GetType());
            }
        }

        [TestMethod]
        public void CanSerializeBinaryTest()
        {
            foreach (var textSerializer in CreateTextSerializers())
            {
                var serializer = textSerializer;
                Assert.IsFalse(serializer.CanSerializeBinary, "'{0}' was not expected to serialize binary.", textSerializer.GetType());
            }

            foreach (var binSerializer in CreateBinarySerializers())
            {
                var serializer = binSerializer;
                Assert.IsTrue(serializer.CanSerializeBinary, "'{0}' was expected to serialize binary.", serializer.GetType());
            }
        }

        [TestMethod]
        public void SerializeDeserializeTextTest()
        {
            foreach (var textSerializer in CreateTextSerializers())
            {
                var serializer = textSerializer;
                foreach (var expected in SerializerTestData.TestExpressions)
                {
                    var text = serializer.SerializeText(expected);

                    TestContext.WriteLine("{0} serializes to text with length {1}: {2}", expected, text.Length, text);

                    var actual = serializer.DeserializeText(text);

                    if (expected == null)
                    {
                        Assert.IsNull(actual, "Input expression was null, but output is {0} for '{1}'", actual, textSerializer.GetType());
                        continue;
                    }
                    Assert.IsNotNull(actual, "Input expression was {0}, but output is null for '{1}'", expected, textSerializer.GetType());
                    ExpressionAssert.AreEqual(expected, actual);
                }
            }
        }

        [TestMethod]
        public void SerializeDeserializeBinaryTest()
        {
            foreach (var binSerializer in CreateBinarySerializers())
            {
                var serializer = binSerializer;
                foreach (var expected in SerializerTestData.TestExpressions)
                {
                    var bytes = serializer.SerializeBinary(expected);

                    TestContext.WriteLine("{0} serializes to bytes with length {1}", expected, bytes.Length);

                    var actual = serializer.DeserializeBinary(bytes);

                    if (expected == null)
                    {
                        Assert.IsNull(actual, "Input expression was null, but output is {0} for '{1}'", actual, binSerializer.GetType());
                        continue;
                    }
                    Assert.IsNotNull(actual, "Input expression was {0}, but output is null for '{1}'", expected, binSerializer.GetType());
                    ExpressionAssert.AreEqual(expected, actual);
                }
            }
        }

        [TestMethod]
        public void SerializeDeserializeBinaryComplexExpressionWithCompileTest()
        {
            foreach (var binSerializer in CreateBinarySerializers())
            {
                var serializer = binSerializer;

                var expected = (Expression<Func<Bar, bool>>)(p => p.LastName == "Miller" && p.FirstName.StartsWith("M"));
                expected.Compile();

                var bytes = serializer.SerializeBinary(expected);
                TestContext.WriteLine("{0} serializes to bytes with length {1}", expected, bytes.Length);

                var actual = (Expression<Func<Bar, bool>>)serializer.DeserializeBinary(bytes);
                Assert.IsNotNull(actual, "Input expression was {0}, but output is null for '{1}'", expected, binSerializer.GetType());
                ExpressionAssert.AreEqual(expected, actual);

                actual.Compile();
            }
        }

        [TestMethod]
        public void NullableDecimalTest()
        {
            foreach (var textSerializer in CreateTextSerializers())
            {
                var serializer = textSerializer;
                var expected = Expression.Constant(0m, typeof(decimal?));

                var text = serializer.SerializeText(expected);

                TestContext.WriteLine("{0} serializes to text with length {1}: {2}", expected, text.Length, text);

                var actual = serializer.DeserializeText(text);
                Assert.IsNotNull(actual, "Input expression was {0}, but output is null for '{1}'", expected, textSerializer.GetType());
                ExpressionAssert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void SerializeNewObjWithoutParameters()
        {
            var serializer = new JsonSerializer();

            Expression<Func<List<int>, List<int>>> exp = l => new List<int>();

            var result = serializer.SerializeGeneric(exp);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SerializeFuncExpressionsWithoutParameters()
        {
            var serializer = new JsonSerializer();

            Expression<Func<bool>> exp = () => false;

            var result = serializer.SerializeGeneric(exp);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SerializeDeserializeGuidValueAsJson()
        {
            SerializeDeserializeExpressionAsText(CreateGuidExpression(), new JsonSerializer());
        }

        [TestMethod]
        public void SerializeDeserializeGuidValueAsXml()
        {
            SerializeDeserializeExpressionAsText(CreateGuidExpression(), new XmlSerializer());
        }

        [TestMethod]
        public void SerializeDeserializeGuidValueAsBinary()
        {
            SerializeDeserializeExpressionAsBinary(CreateGuidExpression(), new BinarySerializer());
        }

        [TestMethod]
        public void ExpressionWithConstantDateTimeAsJson()
        {
            SerializeDeserializeExpressionAsText(CreateConstantDateTimeExpression(), new JsonSerializer());
        }

        [TestMethod]
        public void ExpressionWithConstantDateTimeAsXml()
        {
            SerializeDeserializeExpressionAsText(CreateConstantDateTimeExpression(), new XmlSerializer());
        }

        [TestMethod]
        public void ExpressionWithConstantDateTimeOffsetAsBinary()
        {
            SerializeDeserializeExpressionAsBinary(CreateConstantDateTimeOffsetExpression(), new BinarySerializer());
        }

        [TestMethod]
        public void ExpressionWithConstantDateTimeOffsetAsJson()
        {
            SerializeDeserializeExpressionAsText(CreateConstantDateTimeOffsetExpression(), new JsonSerializer());
        }

        [TestMethod]
        public void ExpressionWithConstantDateTimeOffsetAsXml()
        {
            SerializeDeserializeExpressionAsText(CreateConstantDateTimeOffsetExpression(), new XmlSerializer());
        }

        [TestMethod]
        public void ExpressionWithConstantDateTimeAsBinary()
        {
            SerializeDeserializeExpressionAsBinary(CreateConstantDateTimeExpression(), new BinarySerializer());
        }

        [TestMethod]
        public void ExpressionWithConstantTypeAsJson()
        {
            SerializeDeserializeExpressionAsText(CreateConstantTypeExpression(), new JsonSerializer());
        }

        [TestMethod]
        public void ExpressionWithConstantTypeAsXml()
        {
            SerializeDeserializeExpressionAsText(CreateConstantTypeExpression(), new XmlSerializer());
        }

        [TestMethod]
        public void ExpressionWithConstantTypeAsBinary()
        {
            SerializeDeserializeExpressionAsBinary(CreateConstantTypeExpression(), new BinarySerializer());
        }

        private static ConstantExpression CreateConstantDateTimeExpression()
        {
            return Expression.Constant(DateTime.Today);
        }

        private static ConstantExpression CreateConstantDateTimeOffsetExpression()
        {
            return Expression.Constant(DateTimeOffset.Now);
        }

        private static Expression<Func<Guid>> CreateGuidExpression()
        {
            var guidValue = Guid.NewGuid();
            return () => guidValue;
        }

        private static ConstantExpression CreateConstantTypeExpression()
        {
            return Expression.Constant(typeof(string));
        }

        private static Expression SerializeDeserializeExpressionAsText(Expression expression, IGenericSerializer<string> serializer)
        {
            var expressionSerializer = serializer;
            var serialized = expressionSerializer.SerializeGeneric(expression);

            return expressionSerializer.DeserializeGeneric(serialized);
        }

        private static Expression SerializeDeserializeExpressionAsBinary(Expression expression, IGenericSerializer<byte[]> serializer)
        {
            var expressionSerializer = serializer;
            var serialized = expressionSerializer.SerializeGeneric(expression);

            return expressionSerializer.DeserializeGeneric(serialized);
        }

        private static IEnumerable<ITextTypeSerializer> CreateTextSerializers()
        {
            return new ITextTypeSerializer[] { new JsonSerializer(), new XmlSerializer() };
        }

        private static IEnumerable<IBinaryTypeSerializer> CreateBinarySerializers()
        {
#if WINDOWS_UWP 
            return new IBinaryTypeSerializer[] { new BinarySerializer() };
#else
            return new IBinaryTypeSerializer[] { new BinarySerializer(), new BinaryFormatterSerializerGeneric() };
#endif
        }
    }
}
