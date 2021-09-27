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
            SerializeInternal(new BinarySerializer());
            SerializeInternal(new XmlSerializer());
            SerializeInternal(new JsonSerializer());
            SerializeInternal(new BinarySerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
            SerializeInternal(new XmlSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
            SerializeInternal(new JsonSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
        }

        [TestMethod]
        public void SerializeDeserialize()
        {
            SerializeDeserializeInternal(new BinarySerializer());
            SerializeDeserializeInternal(new XmlSerializer());
            SerializeDeserializeInternal(new JsonSerializer());
            SerializeDeserializeInternal(new BinarySerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
            SerializeDeserializeInternal(new XmlSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
            SerializeDeserializeInternal(new JsonSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
        }

        private static void SerializeInternal<T>(IGenericSerializer<T> serializer)
        {
            var list = new [] { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var value = serializer.SerializeGeneric(expression);

            Assert.IsNotNull(value);
        }

        private static void SerializeDeserializeInternal<T>(IGenericSerializer<T> serializer)
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

        public class Test
        {
            public string Code { get; set; }
        }
    }
}
