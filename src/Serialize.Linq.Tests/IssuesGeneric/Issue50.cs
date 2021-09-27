using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Internals;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.IssuesGeneric
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/50
    /// </summary>
    [TestClass]
    public class Issue50
    {
        [TestMethod]
        public void SerializeArrayAsJson()
        {
            var list = new [] { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new JsonSerializer();
            var value = serializer.SerializeGeneric(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializeArrayAsBinary()
        {
            var list = new[] { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new BinarySerializer();
            var value = serializer.SerializeGeneric(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializeListAsJson()
        {
            var list = new List<string> { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new JsonSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            };
            var value = serializer.SerializeGeneric(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializeListAsBinary()
        {
            var list = new List<string> { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new BinarySerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            };
            var value = serializer.SerializeGeneric(expression);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializeDeserializeArrayAsJson()
        {
            var list = new[] { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new JsonSerializer();
            var value = serializer.SerializeGeneric(expression);

            var actualExpression = (Expression<Func<Test, bool>>)serializer.DeserializeGeneric(value);
            var func = actualExpression.Compile();


            Assert.IsTrue(func(new Test { Code = "one" }), "one failed.");
            Assert.IsTrue(func(new Test { Code = "two" }), "two failed.");
            Assert.IsFalse(func(new Test { Code = "three" }), "three failed.");
        }

        [TestMethod]
        public void SerializeDeserializeArrayAsBinary()
        {
            var list = new[] { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new BinarySerializer();
            var value = serializer.SerializeGeneric(expression);

            var actualExpression = (Expression<Func<Test, bool>>)serializer.DeserializeGeneric(value);
            var func = actualExpression.Compile();

            Assert.IsTrue(func(new Test { Code = "one" }), "one failed.");
            Assert.IsTrue(func(new Test { Code = "two" }), "two failed.");
            Assert.IsFalse(func(new Test { Code = "three" }), "three failed.");
        }

        public class Test
        {
            public string Code { get; set; }
        }
    }
}
