using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Extensions;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests
{
    [TestClass]
    public class JsonSerializerTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void SerializeJsonTest()
        {
            foreach (var item in SerializerTestData.TestItems)
            {
                var json = item.Expression.ToJson();
                Assert.AreEqual(item.Json, json);
            }
        }
    }
}
