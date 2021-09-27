﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.IssuesGeneric
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/96
    /// </summary>
    [TestClass]
    public class Issue96
    {
        public class Test
        {
            public string Code { get; set; }
        }

        [TestMethod]
        public void SerializeDeserializeList()
        {
            SerializeDeserializeListInternal( new BinarySerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
            SerializeDeserializeListInternal(new JsonSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            });
        }

        private void SerializeDeserializeListInternal<T>(IGenericSerializer<T> serializer)
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
    }
}