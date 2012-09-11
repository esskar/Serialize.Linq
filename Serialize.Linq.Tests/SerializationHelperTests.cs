using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Tests
{
    [TestClass]
    public class SerializationHelperTests
    {
        private static readonly string __serializationHelperTypeText = "Serialize.Linq.Internals.SerializationHelper, Serialize.Linq, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        private static readonly string __serializationHelperSerializeMethodText = __serializationHelperTypeText + Environment.NewLine + "System.String SerializeMethod(System.Reflection.MethodInfo)";

        [TestMethod]
        public void SerializeTypeTest()
        {
            var actual = SerializationHelper.SerializeType(typeof(SerializationHelper));
            Assert.AreEqual(__serializationHelperTypeText, actual);
        }

        [TestMethod]
        public void SerializeNullTypeTest()
        {
            var actual = SerializationHelper.SerializeType(null);
            Assert.AreSame(null, actual);
        }

        [TestMethod]
        public void DeserializeTypeTest()
        {
            var actual = SerializationHelper.DeserializeType(__serializationHelperTypeText);
            Assert.AreSame(typeof(SerializationHelper), actual);
        }

        [TestMethod]
        public void DeserializeNullTypeTest()
        {
            var actual = SerializationHelper.DeserializeType(null);
            Assert.AreSame(null, actual);
        }

        [TestMethod]
        public void SerializeDeserializeAllSystemTypesTest()
        {
            foreach (var type in typeof(string).Assembly.GetTypes())            
                Assert.AreSame(type, SerializationHelper.DeserializeType(SerializationHelper.SerializeType(type)));            
        }

        [TestMethod]
        public void SerializeMethodTest()
        {
            var method = typeof(SerializationHelper).GetMethod("SerializeMethod");
            var actual = SerializationHelper.SerializeMethod(method);
            Assert.AreEqual(__serializationHelperSerializeMethodText, actual);
        }

        [TestMethod]
        public void DeserializeMethodTest()
        {
            var method = typeof(SerializationHelper).GetMethod("SerializeMethod");
            var actual = SerializationHelper.DeserializeMethod(__serializationHelperSerializeMethodText);
            Assert.AreEqual(method, actual);
        }
    }
}
