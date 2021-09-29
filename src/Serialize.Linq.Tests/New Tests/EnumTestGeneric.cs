using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.NewTests
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/105
    /// Test and fix provided by https://github.com/oahrens
    /// </summary>
    [TestClass]
    public class EnumTestsGeneric
    {

        [TestMethod]
        public void SerializeDeserializeEnum()
        {
            SerializeDeserializeEnumInternal(new BinarySerializer());
            SerializeDeserializeEnumInternal(new XmlSerializer());
            SerializeDeserializeEnumInternal(new JsonSerializer());
        }

        private static void SerializeDeserializeEnumInternal<T>(IGenericSerializer<T> serializer)
        {
            var two = TestEnum.Two;
            Expression<Func<TestEnum, TestEnum>> expression = en => en | two;
            // prevents SerializationException in version 2.0.0.0; no longer needed in the new version:
            // serializer.AddKnownType(typeof(TestEnum));
            var value = serializer.SerializeGeneric(expression);
            var actualExpression = (Expression<Func<TestEnum, TestEnum>>)serializer.DeserializeGeneric(value);
            var func = actualExpression.Compile();

            // the next line throws a SerializationException in version 2.0.0.0 if the 'prevents' line is commented out
            Assert.IsTrue(func(TestEnum.One) == TestEnum.Three, "Enum 'Three' failed.");
            // the next line throws a SerializationException in version 2.0.0.0 if the 'prevents' line is commented out
            Assert.IsFalse(func(TestEnum.One) == TestEnum.Four, "Enum 'Four' failed.");
        }

        [Flags]
        private enum TestEnum : int
        {
            None = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4
        }
    }
}