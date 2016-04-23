using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/50
    /// </summary>
    
    public class Issue50
    {
        [Fact]
        public void SerializeArrayAsJson()
        {
            var list = new [] { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            Assert.NotNull(value);
        }

        [Fact]
        public void SerializeArrayAsBinary()
        {
            var list = new[] { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new ExpressionSerializer(new BinarySerializer());
            var value = serializer.SerializeBinary(expression);

            Assert.NotNull(value);
        }

        [Fact]
        public void SerializeListAsJson()
        {
            var list = new List<string> { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new ExpressionSerializer(new JsonSerializer())
            {
                AutoAddKnownTypesAsListTypes = true
            };
            var value = serializer.SerializeText(expression);

            Assert.NotNull(value);
        }

        [Fact]
        public void SerializeListAsBinary()
        {
            var list = new List<string> { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new ExpressionSerializer(new BinarySerializer())
            {
                AutoAddKnownTypesAsListTypes = true
            };
            var value = serializer.SerializeBinary(expression);

            Assert.NotNull(value);
        }

        [Fact]
        public void SerializeDeserializeArrayAsJson()
        {
            var list = new[] { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(expression);

            var actualExpression = (Expression<Func<Test, bool>>)serializer.DeserializeText(value);
            var func = actualExpression.Compile();


            Assert.True(func(new Test { Code = "one" }), "one failed.");
            Assert.True(func(new Test { Code = "two" }), "two failed.");
            Assert.False(func(new Test { Code = "three" }), "three failed.");
        }

        [Fact]
        public void SerializeDeserializeArrayAsBinary()
        {
            var list = new[] { "one", "two" };
            Expression<Func<Test, bool>> expression = test => list.Contains(test.Code);

            var serializer = new ExpressionSerializer(new BinarySerializer());
            var value = serializer.SerializeBinary(expression);

            var actualExpression = (Expression<Func<Test, bool>>)serializer.DeserializeBinary(value);
            var func = actualExpression.Compile();

            Assert.True(func(new Test { Code = "one" }), "one failed.");
            Assert.True(func(new Test { Code = "two" }), "two failed.");
            Assert.False(func(new Test { Code = "three" }), "three failed.");
        }

        public class Test
        {
            public string Code { get; set; }
        }
    }
}
