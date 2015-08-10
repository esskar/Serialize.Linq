using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    [TestClass]
    public class Issue54
    {
        public int PublicField = 42;
        private int _privateField = 42;
        protected int ProtectedField = 42;
        internal int InternalField = 42;
        protected internal int ProtectedInternalField = 42;

        private static readonly int _privateStaticField = 42;
        public static int PublicStaticField = 42;
        protected static int ProtectedStaticField = 42;
        internal static int InternalStaticField = 42;
        protected internal static int ProtectedInternalStaticField = 42;

        [TestMethod]
        public void SerializePublicField()
        {
            TestExpression(test => test.IntProperty == PublicField);
        }

        [TestMethod]
        public void SerializePrivateField()
        {
            TestExpression(test => test.IntProperty == _privateField);
        }

        [TestMethod]
        public void SerializeProtectedField()
        {
            TestExpression(test => test.IntProperty == ProtectedField);
        }

        [TestMethod]
        public void SerializeInternalField()
        {
            TestExpression(test => test.IntProperty == InternalField);
        }

        [TestMethod]
        public void SerializeProtectedInternalField()
        {
            TestExpression(test => test.IntProperty == ProtectedInternalField);
        }

        [TestMethod]
        public void SerializePublicStaticField()
        {
            TestExpression(test => test.IntProperty == PublicStaticField);
        }

        [TestMethod]
        public void SerializePrivateStaticField()
        {
            TestExpression(test => test.IntProperty == _privateStaticField);
        }

        [TestMethod]
        public void SerializeProtectedStaticField()
        {
            TestExpression(test => test.IntProperty == ProtectedStaticField);
        }

        [TestMethod]
        public void SerializeInternalStaticField()
        {
            TestExpression(test => test.IntProperty == InternalStaticField);
        }

        [TestMethod]
        public void SerializeProtectedInternalStaticField()
        {
            TestExpression(test => test.IntProperty == ProtectedInternalStaticField);
        }

        private void TestExpression(Expression<Func<Test, bool>> expression)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            Assert.IsNotNull(value);
        }

        public class Test
        {
            public int IntProperty { get; set; }
        }
    }
}
