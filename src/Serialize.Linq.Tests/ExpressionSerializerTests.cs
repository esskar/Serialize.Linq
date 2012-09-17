using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests
{
    [TestClass]
    public class ExpressionSerializerTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void SerializeToTextJsonTest()
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            foreach (var item in SerializerTestData.TestItems)
            {
                var text = serializer.SerializeToText(item.Expression);
                var expression = serializer.DeserializeFromText(text);

                if(item.Expression == null)
                    Assert.IsNull(expression, "Input expression was null, but output is {0}", expression);
                else
                    Assert.IsNotNull(expression, "Input expression was {0}, but output is null", item.Expression);
            }
        }

        [TestMethod]
        public void SerializeToTextXmlTest()
        {
            var serializer = new ExpressionSerializer(new XmlSerializer());
            foreach (var item in SerializerTestData.TestItems)
            {
                var text = serializer.SerializeToText(item.Expression);
                var expression = serializer.DeserializeFromText(text);

                if(item.Expression == null)
                    Assert.IsNull(expression, "Input expression was null, but output is {0}", expression);
                else
                    Assert.IsNotNull(expression, "Input expression was {0}, but output is null", item.Expression);
            }
        }
    }
}
