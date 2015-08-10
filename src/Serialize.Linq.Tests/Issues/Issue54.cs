using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    [TestClass]
    public class Issue54
    {
        // ReSharper disable InconsistentNaming
        public int _publicField = 42;
        private int _privateField = 42;
        protected int _protectedField = 42;
        internal int _internalField = 42;
        protected internal int _protectedInternalField = 42;

        private static int _privateStaticField = 42;
        public static int _publicStaticField = 42;
        protected static int _protectedStaticField = 42;
        internal static int _internalStaticField = 42;
        protected internal static int _protectedInternalStaticField = 42;
        // ReSharper restore InconsistentNaming

        [TestMethod]
        public void SerializePublicField()
        {
            TestExpression(test => test.IntProperty == _publicField);
        }

        [TestMethod]
        public void SerializePrivateField()
        {
            TestExpression(test => test.IntProperty == _privateField);
        }

        [TestMethod]
        public void SerializeProtectedField()
        {
            TestExpression(test => test.IntProperty == _protectedField);
        }

        [TestMethod]
        public void SerializeInternalField()
        {
            TestExpression(test => test.IntProperty == _internalField);
        }

        [TestMethod]
        public void SerializeProtectedInternalField()
        {
            TestExpression(test => test.IntProperty == _protectedInternalField);
        }

        [TestMethod]
        public void SerializePublicStaticField()
        {
            TestExpression(test => test.IntProperty == _publicStaticField);
        }

        [TestMethod]
        public void SerializePrivateStaticField()
        {
            TestExpression(test => test.IntProperty == _privateStaticField);
        }

        [TestMethod]
        public void SerializeProtectedStaticField()
        {
            TestExpression(test => test.IntProperty == _protectedStaticField);
        }

        [TestMethod]
        public void SerializeInternalStaticField()
        {
            TestExpression(test => test.IntProperty == _internalStaticField);
        }

        [TestMethod]
        public void SerializeProtectedInternalStaticField()
        {
            TestExpression(test => test.IntProperty == _protectedInternalStaticField);
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
