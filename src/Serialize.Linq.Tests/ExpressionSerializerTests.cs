using System.Collections.Generic;
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
                var serializer = new ExpressionSerializer(textSerializer);
                Assert.IsTrue(serializer.CanSerializeText, "'{0}' was expected to serialize text.", textSerializer.GetType());    
            }            

            foreach (var binSerializer in CreateBinarySerializers())
            {
                var serializer = new ExpressionSerializer(binSerializer);
                Assert.IsFalse(serializer.CanSerializeText, "'{0}' was not expected to serialize text.", serializer.GetType());    
            }            
        }

        [TestMethod]
        public void CanSerializeBinaryTest()
        {
            foreach (var textSerializer in CreateTextSerializers())
            {
                var serializer = new ExpressionSerializer(textSerializer);
                Assert.IsFalse(serializer.CanSerializeBinary, "'{0}' was not expected to serialize binary.", textSerializer.GetType());    
            }            

            foreach (var binSerializer in CreateBinarySerializers())
            {
                var serializer = new ExpressionSerializer(binSerializer);
                Assert.IsTrue(serializer.CanSerializeBinary, "'{0}' was expected to serialize binary.", serializer.GetType());    
            }            
        }

        [TestMethod]
        public void SerializeDeserializeTextTest()
        {
            foreach (var textSerializer in CreateTextSerializers())
            {
                var serializer = new ExpressionSerializer(textSerializer);
                foreach (var expected in SerializerTestData.TestExpressions)
                {
                    var text = serializer.SerializeText(expected);

                    this.TestContext.WriteLine("{0} serializes to text with length {1}: {2}", expected, text.Length, text);

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
                var serializer = new ExpressionSerializer(binSerializer);
                foreach (var expected in SerializerTestData.TestExpressions)
                {
                    var bytes = serializer.SerializeBinary(expected);
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

        private static IEnumerable<ITextSerializer> CreateTextSerializers()
        {
            return new ITextSerializer[] { new JsonSerializer(), new XmlSerializer() };
        }

        private static IEnumerable<IBinarySerializer> CreateBinarySerializers()
        {
            return new IBinarySerializer[] { new BinarySerializer() };
        }        
    }
}
