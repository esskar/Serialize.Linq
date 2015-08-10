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
            Expression<Func<Test, bool>> expression = test => test.IntProperty == _publicField;

            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializePrivateField()
        {
            Expression<Func<Test, bool>> expression = test => test.IntProperty == _privateField;

            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializeProtectedField()
        {
            Expression<Func<Test, bool>> expression = test => test.IntProperty == _protectedField;

            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializeInternalField()
        {
            Expression<Func<Test, bool>> expression = test => test.IntProperty == _internalField;

            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializeProtectedInternalField()
        {
            Expression<Func<Test, bool>> expression = test => test.IntProperty == _protectedInternalField;

            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializePublicStaticField()
        {
            Expression<Func<Test, bool>> expression = test => test.IntProperty == _publicStaticField;

            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializePrivateStaticField()
        {
            Expression<Func<Test, bool>> expression = test => test.IntProperty == _privateStaticField;

            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializeProtectedStaticField()
        {
            Expression<Func<Test, bool>> expression = test => test.IntProperty == _protectedStaticField;

            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializeInternalStaticField()
        {
            Expression<Func<Test, bool>> expression = test => test.IntProperty == _internalStaticField;

            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializeProtectedInternalStaticField()
        {
            Expression<Func<Test, bool>> expression = test => test.IntProperty == _protectedInternalStaticField;

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
