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
    public class ExpressionSerializerTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CanSerializeTextTest()
        {
            foreach (var textSerializer in CreateTextSerializers())
            {
#pragma warning disable CS0618 // type or member is obsolete
                var serializer = new ExpressionSerializer(textSerializer);
#pragma warning restore CS0618 // type or member is obsolete
                Assert.IsTrue(serializer.CanSerializeText, "'{0}' was expected to serialize text.", textSerializer.GetType());
            }

            foreach (var binSerializer in CreateBinarySerializers())
            {
#pragma warning disable CS0618 // type or member is obsolete
                var serializer = new ExpressionSerializer(binSerializer);
#pragma warning restore CS0618 // type or member is obsolete
                Assert.IsFalse(serializer.CanSerializeText, "'{0}' was not expected to serialize text.", serializer.GetType());
            }
        }

        [TestMethod]
        public void CanSerializeBinaryTest()
        {
            foreach (var textSerializer in CreateTextSerializers())
            {
#pragma warning disable CS0618 // type or member is obsolete
                var serializer = new ExpressionSerializer(textSerializer);
#pragma warning restore CS0618 // type or member is obsolete
                Assert.IsFalse(serializer.CanSerializeBinary, "'{0}' was not expected to serialize binary.", textSerializer.GetType());
            }

            foreach (var binSerializer in CreateBinarySerializers())
            {
#pragma warning disable CS0618 // type or member is obsolete
                var serializer = new ExpressionSerializer(binSerializer);
#pragma warning restore CS0618 // type or member is obsolete
                Assert.IsTrue(serializer.CanSerializeBinary, "'{0}' was expected to serialize binary.", serializer.GetType());
            }
        }

        [TestMethod]
        public void SerializeDeserializeTextTest()
        {
            foreach (var textSerializer in CreateTextSerializers())
            {
#pragma warning disable CS0618 // type or member is obsolete
                var serializer = new ExpressionSerializer(textSerializer);
#pragma warning restore CS0618 // type or member is obsolete
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
#pragma warning disable CS0618 // type or member is obsolete
                var serializer = new ExpressionSerializer(binSerializer);
#pragma warning restore CS0618 // type or member is obsolete
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
#pragma warning disable CS0618 // type or member is obsolete
                var serializer = new ExpressionSerializer(binSerializer);
#pragma warning restore CS0618 // type or member is obsolete

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
#pragma warning disable CS0618 // type or member is obsolete
                var serializer = new ExpressionSerializer(textSerializer);
#pragma warning restore CS0618 // type or member is obsolete
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
#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new JsonSerializer());
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<List<int>, List<int>>> exp = l => new List<int>();

            var result = serializer.SerializeText(exp);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SerializeFuncExpressionsWithoutParameters()
        {
#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new JsonSerializer());
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<bool>> exp = () => false;

            var result = serializer.SerializeText(exp);
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

        private static Expression SerializeDeserializeExpressionAsText(Expression expression, ISerializer serializer)
        {
#pragma warning disable CS0618 // type or member is obsolete
            var expressionSerializer = new ExpressionSerializer(serializer);
#pragma warning restore CS0618 // type or member is obsolete
            var serialized = expressionSerializer.SerializeText(expression);

            return expressionSerializer.DeserializeText(serialized);
        }

        private static Expression SerializeDeserializeExpressionAsBinary(Expression expression, ISerializer serializer)
        {
#pragma warning disable CS0618 // type or member is obsolete
            var expressionSerializer = new ExpressionSerializer(serializer);
#pragma warning restore CS0618 // type or member is obsolete
            var serialized = expressionSerializer.SerializeBinary(expression);

            return expressionSerializer.DeserializeBinary(serialized);
        }

        private static IEnumerable<ITextSerializer> CreateTextSerializers()
        {
            return new ITextSerializer[] { new JsonSerializer(), new XmlSerializer() };
        }

        private static IEnumerable<IBinarySerializer> CreateBinarySerializers()
        {
#if WINDOWS_UWP 
            return new IBinarySerializer[] { new BinarySerializer() };
#else
            return new IBinarySerializer[] { new BinarySerializer(), new BinaryFormatterSerializer() };
#endif
        }
    }
}
