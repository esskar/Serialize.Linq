using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestInitialize]
        public void Init()
        {
            SetFields(42);
        }

        [TestMethod]
        public void SerializePublicField()
        {
            TestExpression(test => test.IntProperty == PublicField, true);
        }

        [TestMethod]
        public void SerializePrivateField()
        {
            TestExpression(test => test.IntProperty == _privateField, true);
        }

        [TestMethod]
        public void SerializeProtectedField()
        {
            TestExpression(test => test.IntProperty == ProtectedField, true);
        }

        [TestMethod]
        public void SerializeInternalField()
        {
            TestExpression(test => test.IntProperty == InternalField, true);
        }

        [TestMethod]
        public void SerializeProtectedInternalField()
        {
            TestExpression(test => test.IntProperty == ProtectedInternalField, true);
        }

        [TestMethod]
        public void SerializePublicStaticField()
        {
            TestExpression(test => test.IntProperty == PublicStaticField, false);
        }

        [TestMethod]
        public void SerializePrivateStaticField()
        {
            TestExpression(test => test.IntProperty == _privateStaticField, true);
        }

        [TestMethod]
        public void SerializeProtectedStaticField()
        {
            TestExpression(test => test.IntProperty == ProtectedStaticField, false);
        }

        [TestMethod]
        public void SerializeInternalStaticField()
        {
            TestExpression(test => test.IntProperty == InternalStaticField, false);
        }

        [TestMethod]
        public void SerializeProtectedInternalStaticField()
        {
            TestExpression(test => test.IntProperty == ProtectedInternalStaticField, false);
        }

        private void TestExpression(Expression<Func<Test, bool>> expression, bool shouldSerializeFieldValue)
        {
            // Initialize fields
            SetFields(42);

            // Serialize expression
            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            // Reset fields
            SetFields(-1);

            // Deserialize expression
            var actualExpression = (Expression<Func<Test, bool>>)serializer.DeserializeText(value);
            var func = actualExpression.Compile();

            // Set expected value.
            // For instance fields and for private static fields - the initial value is expected.
            // For other static fields - the actual one.
            int expectedValue = shouldSerializeFieldValue ? 42 : -1;

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
    }
}
