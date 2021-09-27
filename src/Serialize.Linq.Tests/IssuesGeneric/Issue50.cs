using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Interfaces;
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
        public void Serialize()
        {
            SerializeArrayInternal(new BinarySerializer());
            SerializeArrayInternal(new XmlSerializer());
            SerializeArrayInternal(new JsonSerializer());
            SerializeListInternal(new BinarySerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
            SerializeListInternal(new XmlSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
            SerializeListInternal(new JsonSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
        }

        [TestMethod]
        public void SerializeDeserialize()
        {
            SerializeDeserializeArrayInternal(new BinarySerializer());
            SerializeDeserializeArrayInternal(new XmlSerializer());
            SerializeDeserializeArrayInternal(new JsonSerializer());
            SerializeDeserializeListInternal(new BinarySerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
            SerializeDeserializeListInternal(new XmlSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
            SerializeDeserializeListInternal(new JsonSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
        }

        private static void SerializeArrayInternal<T>(IGenericSerializer<T> serializer)
        {
            var list = new [] { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var value = serializer.SerializeGeneric(expression);

            Assert.IsNotNull(value);
        }

        private static void SerializeListInternal<T>(IGenericSerializer<T> serializer)
        {
            var list = new List<string> { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);
            var value = serializer.SerializeGeneric(expression);

            Assert.IsNotNull(value);
        }

        private static void SerializeDeserializeArrayInternal<T>(IGenericSerializer<T> serializer)
        {
            var list = new[] { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var value = serializer.SerializeGeneric(expression);

            var actualExpression = (Expression<Func<Test, bool>>)serializer.DeserializeGeneric(value);
            var func = actualExpression.Compile();

            Assert.IsTrue(func(new Test { Code = "one" }), "one failed.");
            Assert.IsTrue(func(new Test { Code = "two" }), "two failed.");
            Assert.IsFalse(func(new Test { Code = "three" }), "three failed.");
        }

        private static void SerializeDeserializeListInternal<T>(IGenericSerializer<T> serializer)
        {
            var list = new List<string> { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

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
