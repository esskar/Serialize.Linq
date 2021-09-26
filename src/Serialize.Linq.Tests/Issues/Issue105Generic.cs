using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Internals;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/105
    /// Test and fix provided by https://github.com/OlegNadymov THX!!!
    /// </summary>
    [TestClass]
    public class Issue105Generic
    {
        public class Test
        {
            public Guid Id { get; set; }
        }

        [TestMethod]
        public void SerializeDeserializeListAsBinary()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();
            var list = new List<Guid> { guid1, guid2 };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Id);

            var serializer = new BinarySerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            };
            var value = serializer.SerializeGeneric(expression);

            var actualExpression = (Expression<Func<Test, bool>>)serializer.DeserializeGeneric(value);
            var func = actualExpression.Compile();

            Assert.IsTrue(func(new Test { Id = guid1 }), "one failed.");
            Assert.IsTrue(func(new Test { Id = guid2 }), "two failed.");
            Assert.IsFalse(func(new Test { Id = guid3 }), "three failed.");
        }

        [TestMethod]
        public void SerializeDeserializeListAsJson()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();
            var list = new List<Guid> { guid1, guid2 };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Id);

            var serializer = new JsonSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            };
            var value = serializer.SerializeGeneric(expression);

            var actualExpression = (Expression<Func<Test, bool>>)serializer.DeserializeGeneric(value);
            var func = actualExpression.Compile();

            Assert.IsTrue(func(new Test { Id = guid1 }), "one failed.");
            Assert.IsTrue(func(new Test { Id = guid2 }), "two failed.");
            Assert.IsFalse(func(new Test { Id = guid3 }), "three failed.");
        }


        [TestMethod]
        public void SerializeDeserializeListAsXml()
        {
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guid3 = Guid.NewGuid();
            var list = new List<Guid> { guid1, guid2 };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Id);

            var serializer = new XmlSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            };
            var value = serializer.SerializeGeneric(expression);

            var actualExpression = (Expression<Func<Test, bool>>)serializer.DeserializeGeneric(value);
            var func = actualExpression.Compile();

            Assert.IsTrue(func(new Test { Id = guid1 }), "one failed.");
            Assert.IsTrue(func(new Test { Id = guid2 }), "two failed.");
            Assert.IsFalse(func(new Test { Id = guid3 }), "three failed.");
        }
    }
}