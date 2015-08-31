using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Factories;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    [TestClass]
    public class Issue54
    {
        public int PublicField;
        private int _privateField;
        protected int ProtectedField;
        internal int InternalField;
        protected internal int ProtectedInternalField;

        private static int _privateStaticField;
        public static int PublicStaticField;
        protected static int ProtectedStaticField;
        internal static int InternalStaticField;
        protected internal static int ProtectedInternalStaticField;

        [TestMethod]
        public void SerializePublicField()
        {
            TestExpression(test => test.IntProperty == PublicField, ReadFieldOn.Serialization);
        }

        [TestMethod]
        public void SerializePrivateField()
        {
            TestExpression(test => test.IntProperty == _privateField, ReadFieldOn.Serialization);
        }

        [TestMethod]
        public void SerializeProtectedField()
        {
            TestExpression(test => test.IntProperty == ProtectedField, ReadFieldOn.Serialization);
        }

        [TestMethod]
        public void SerializeInternalField()
        {
            TestExpression(test => test.IntProperty == InternalField, ReadFieldOn.Serialization);
        }

        [TestMethod]
        public void SerializeProtectedInternalField()
        {
            TestExpression(test => test.IntProperty == ProtectedInternalField, ReadFieldOn.Serialization);
        }

        [TestMethod]
        public void SerializePublicStaticField()
        {
            TestExpression(test => test.IntProperty == PublicStaticField, ReadFieldOn.Execution);
        }

        [TestMethod]
        public void SerializePrivateStaticField()
        {
            TestExpression(test => test.IntProperty == _privateStaticField, ReadFieldOn.Serialization);
        }

        [TestMethod]
        public void SerializeProtectedStaticField()
        {
            TestExpression(test => test.IntProperty == ProtectedStaticField, ReadFieldOn.Execution);
        }

        [TestMethod]
        public void SerializeInternalStaticField()
        {
            TestExpression(test => test.IntProperty == InternalStaticField, ReadFieldOn.Execution);
        }

        [TestMethod]
        public void SerializeProtectedInternalStaticField()
        {
            TestExpression(test => test.IntProperty == ProtectedInternalStaticField, ReadFieldOn.Execution);
        }

        private void TestExpression(Expression<Func<Test, bool>> expression, ReadFieldOn readFieldOn)
        {
            var initialValue = 42;
            var actualValue = -1;

            // Initialize fields
            SetFields(initialValue);

            // Serialize expression
            var settings = new FactorySettings
            {
                AllowPrivateFieldAccess = true
            };
            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression, settings);

            // Modify fields
            SetFields(actualValue);

            // Deserialize expression
            var actualExpression = (Expression<Func<Test, bool>>)serializer.DeserializeText(value, new ExpressionContext { AllowPrivateFieldAccess = true });
            var func = actualExpression.Compile();

            // Set expected value.
            int expectedValue = readFieldOn == ReadFieldOn.Serialization
                ? initialValue
                : actualValue;

            // Assert
            Assert.IsTrue(func(new Test { IntProperty = expectedValue }));
        }

        public void SetFields(int value)
        {
            PublicField = value;
            _privateField = value;
            ProtectedField = value;
            InternalField = value;
            ProtectedInternalField = value;
            _privateStaticField = value;
            PublicStaticField = value;
            ProtectedStaticField = value;
            InternalStaticField = value;
            ProtectedInternalStaticField = value;
        }

        public class Test
        {
            public int IntProperty { get; set; }
        }

        public enum ReadFieldOn
        {
            Serialization,
            Execution
        }
    }
}
