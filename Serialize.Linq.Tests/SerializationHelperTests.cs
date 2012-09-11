using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Internals;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests
{
    [TestClass]
    public class SerializationHelperTests
    {
        private static readonly string __barTypeText = typeof(Bar).AssemblyQualifiedName;
        private static readonly string __barGetNameText = __barTypeText + Environment.NewLine + "System.String GetName()";
        private static readonly string __barNameText = __barTypeText + Environment.NewLine + "System.String Name";
        private static readonly string __barConstructorText = __barTypeText + Environment.NewLine + "Void .ctor()";

        [TestMethod]
        public void SerializeTypeTest()
        {
            var actual = SerializationHelper.SerializeType(typeof(Bar));
            Assert.AreEqual(__barTypeText, actual);
        }

        [TestMethod]
        public void SerializeNullTypeTest()
        {
            var actual = SerializationHelper.SerializeType(null);
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void DeserializeTypeTest()
        {
            var actual = SerializationHelper.DeserializeType(__barTypeText);
            Assert.AreEqual(typeof(Bar), actual);
        }

        [TestMethod]
        public void DeserializeNullTypeTest()
        {
            var actual = SerializationHelper.DeserializeType(null);
            Assert.AreEqual(null, actual);
        }

        [TestMethod]
        public void SerializeDeserializeAllSystemTypesTest()
        {
            foreach (var expected in typeof(string).Assembly.GetTypes())            
                Assert.AreEqual(expected, SerializationHelper.DeserializeType(SerializationHelper.SerializeType(expected)));            
        }

        [TestMethod]
        public void SerializeMethodTest()
        {
            var method = typeof(Bar).GetMethod("GetName");
            var actual = SerializationHelper.SerializeMethod(method);
            Assert.AreEqual(__barGetNameText, actual);
        }

        [TestMethod]
        public void DeserializeMethodTest()
        {
            var expected = typeof(Bar).GetMethod("GetName");
            var actual = SerializationHelper.DeserializeMethod(__barGetNameText);
            Assert.AreEqual(expected, actual);
        }        

        [TestMethod]
        public void SerializeMemberTest()
        {
            var member = typeof(Bar).GetProperty("Name");
            var actual = SerializationHelper.SerializeMember(member);
            Assert.AreEqual(__barNameText, actual);
        }

        [TestMethod]
        public void DeserializeMemberTest()
        {
            var expected = typeof(Bar).GetProperty("Name");
            var actual = SerializationHelper.DeserializeMember(__barNameText);
            Assert.AreEqual(expected, actual);
        }        

        [TestMethod]
        public void SerializeConstructorTest()
        {
            var constructor = typeof(Bar).GetConstructor(Type.EmptyTypes);
            var actual = SerializationHelper.SerializeConstructor(constructor);
            Assert.AreEqual(__barConstructorText, actual);
        }

        [TestMethod]
        public void DeserializeConstructorTest()
        {
            var expected = typeof(Bar).GetConstructor(Type.EmptyTypes);
            var actual = SerializationHelper.DeserializeConstructor(__barConstructorText);
            Assert.AreEqual(expected, actual);
        }        
    }
}
