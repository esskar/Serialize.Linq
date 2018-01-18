using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
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
        public void SerializeDeserializeListAsBinary()
        {
            var list = new List<string> { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new ExpressionSerializer(new BinarySerializer())
            {
                AutoAddKnownTypesAsListTypes = true
            };
            var value = serializer.SerializeBinary(expression);

            var actualExpression = (Expression<Func<Test, bool>>)serializer.DeserializeBinary(value);
            var func = actualExpression.Compile();

            Assert.IsTrue(func(new Test { Code = "one" }), "one failed.");
            Assert.IsTrue(func(new Test { Code = "two" }), "two failed.");
            Assert.IsFalse(func(new Test { Code = "three" }), "three failed.");
        }
    }
}
