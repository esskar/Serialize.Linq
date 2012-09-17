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
                foreach (var item in SerializerTestData.TestItems)
                {
                    var text = serializer.SerializeText(item.Expression);
                    var expression = serializer.DeserializeText(text);

                    if (item.Expression == null)
                    {
                        Assert.IsNull(expression, "Input expression was null, but output is {0} for '{1}'", expression, textSerializer.GetType());
                        continue;
                    }
                    Assert.IsNotNull(expression, "Input expression was {0}, but output is null for '{1}'", item.Expression, textSerializer.GetType());
                    Assert.AreEqual(item.Expression.ToString(), expression.ToString());
                }
            }
        }        

        [TestMethod]
        public void SerializeDeserializeBinaryTest()
        {
            foreach (var binSerializer in CreateBinarySerializers())
            {
                var serializer = new ExpressionSerializer(binSerializer);
                foreach (var item in SerializerTestData.TestItems)
                {
                    var text = serializer.SerializeBinary(item.Expression);
                    var expression = serializer.DeserializeBinary(text);

                    if (item.Expression == null)
                    {
                        Assert.IsNull(expression, "Input expression was null, but output is {0} for '{1}'", expression, binSerializer.GetType());
                        continue;
                    }
                    Assert.IsNotNull(expression, "Input expression was {0}, but output is null for '{1}'", item.Expression, binSerializer.GetType());
                    Assert.AreEqual(item.Expression.ToString(), expression.ToString());
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
