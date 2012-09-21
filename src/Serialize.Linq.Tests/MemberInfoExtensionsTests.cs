using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Extensions;
using Serialize.Linq.Tests.Internals;


namespace Serialize.Linq.Tests
{
    [TestClass]
    public class MemberInfoExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void GetReturnTypeOfPropertyTest()
        {
            var actual = typeof(Bar).GetProperty("FirstName").GetReturnType();
            Assert.AreEqual(typeof(string), actual);
        }

        [TestMethod]
        public void GetReturnTypeOfFieldTest()
        {
            var actual = typeof(Bar).GetField("IsFoo").GetReturnType();
            Assert.AreEqual(typeof(bool), actual);
        }

        [TestMethod]
        public void GetReturnTypeOfMethodTest()
        {
            var actual = typeof(Bar).GetMethod("GetName").GetReturnType();
            Assert.AreEqual(typeof(string), actual);
        }
    }
}