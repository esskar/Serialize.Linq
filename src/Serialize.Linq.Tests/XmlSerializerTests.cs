using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Extensions;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests
{
    [TestClass]
    public class XmlSerializerTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void SerializeXmlTest()
        {
            foreach (var item in SerializerTestData.TestItems)
            {
                var xml = item.Expression.ToXml();
                Assert.AreEqual(item.Xml, xml);
            }
        }
    }
}
