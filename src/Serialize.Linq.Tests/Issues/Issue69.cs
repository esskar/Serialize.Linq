using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using Serialize.Linq.Serializers;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests.Issues
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/69
    /// </summary>

    public class Issue69 : IDisposable
    {
        private ExpressionSerializer _jsonExpressionSerializer;

        public Issue69()
        {
            _jsonExpressionSerializer = new ExpressionSerializer(new JsonSerializer());
        }

        [Fact]
        public void JsonSerialzeAndDeserialize1969Utc()
        {
            SerializeAndDeserializeDateTimeJson(new DateTime(1969, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void JsonSerialzeAndDeserialize1969Local()
        {
            SerializeAndDeserializeDateTimeJson(new DateTime(1969, 1, 1, 0, 0, 0, DateTimeKind.Local));
        }

        [Fact]
        public void JsonSerialzeAndDeserialize1970Utc()
        {
            SerializeAndDeserializeDateTimeJson(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void JsonSerialzeAndDeserialize1970Local()
        {
            SerializeAndDeserializeDateTimeJson(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local));
        }

        [Fact]
        public void JsonSerialzeAndDeserialize1971Utc()
        {
            SerializeAndDeserializeDateTimeJson(new DateTime(1971, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void JsonSerialzeAndDeserialize1971Local()
        {
            SerializeAndDeserializeDateTimeJson(new DateTime(1971, 1, 1, 0, 0, 0, DateTimeKind.Local));
        }

        private void SerializeAndDeserializeDateTimeJson(DateTime dt)
        {
            Expression<Func<DateTime>> actual = () => dt;
            actual = actual.Update(Expression.Constant(dt), new List<ParameterExpression>());

            var serialized = _jsonExpressionSerializer.SerializeText(actual);
            var expected = _jsonExpressionSerializer.DeserializeText(serialized);
            ExpressionAssert.AreEqual(expected, actual);
        }

        public void Dispose()
        {
            _jsonExpressionSerializer = null;
        }
    }
}