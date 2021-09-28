using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.NewTests
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/105
    /// Test and fix provided by https://github.com/OlegNadymov THX!!!
    /// </summary>
    [TestClass]
    public class EnumTests
    {

        [TestMethod]
        public void SerializeDeserializeEnumBinary()
        {
            var two = TestEnum.Two;
            Expression<Func<TestEnum, TestEnum>> expression = en => en | two;
#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new BinarySerializer());
#pragma warning restore CS0618 // type or member is obsolete
            // prevents SerializationException in version 2.0.0.0; no longer needed in the new version:
            // serializer.AddKnownType(typeof(TestEnum));
            var value = serializer.SerializeBinary(expression);
            var actualExpression = (Expression<Func<TestEnum, TestEnum>>)serializer.DeserializeBinary(value);
            var func = actualExpression.Compile();

            // the next line throws a SerializationException in version 2.0.0.0 if the 'prevents' line is commented out
            Assert.IsTrue(func(TestEnum.One) == TestEnum.Three, "Enum 'Three' failed.");
            // the next line throws a SerializationException in version 2.0.0.0 if the 'prevents' line is commented out
            Assert.IsFalse(func(TestEnum.One) == TestEnum.Four, "Enum 'Four' failed.");
        }

        [TestMethod]
        public void SerializeDeserializeEnumXml()
        {
            var two = TestEnum.Two;
            Expression<Func<TestEnum, TestEnum>> expression = en => en | two;
#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new XmlSerializer());
#pragma warning restore CS0618 // type or member is obsolete
            // prevents SerializationException in version 2.0.0.0; no longer needed in the new version:
            // serializer.AddKnownType(typeof(TestEnum));
            var value = serializer.SerializeText(expression);
            var actualExpression = (Expression<Func<TestEnum, TestEnum>>)serializer.DeserializeText(value);
            var func = actualExpression.Compile();

            // the next line throws a SerializationException in version 2.0.0.0 if the 'prevents' line is commented out
            Assert.IsTrue(func(TestEnum.One) == TestEnum.Three, "Enum 'Three' failed.");
            // the next line throws a SerializationException in version 2.0.0.0 if the 'prevents' line is commented out
            Assert.IsFalse(func(TestEnum.One) == TestEnum.Four, "Enum 'Four' failed.");
        }

        [TestMethod]
        public void SerializeDeserializeEnumJson()
        {
            var two = TestEnum.Two;
            Expression<Func<TestEnum, TestEnum>> expression = en => en | two;
#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new JsonSerializer());
#pragma warning restore CS0618 // type or member is obsolete
            // prevents SerializationException in version 2.0.0.0; no longer needed in the new version:
            // serializer.AddKnownType(typeof(TestEnum));
            var value = serializer.SerializeText(expression);
            var actualExpression = (Expression<Func<TestEnum, TestEnum>>)serializer.DeserializeText(value);
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