#if DNXCORE50
// skip this test for DNXCORE50 -->
// System.MissingMethodException : Method not found: 'System.Linq.Expressions.Expression`1<!0> System.Linq.Expressions.Expression`1.Update(System.Linq.Expressions.Expression, System.Collections.Generic.IEnumerable`1<System.Linq.Expressions.ParameterExpression>)'.
#else
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
        public void JsonSerializeAndDeserialize1969Utc()
        {
            SerializeAndDeserializeDateTimeJson(new DateTime(1969, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void JsonSerializeAndDeserialize1969Local()
        {
            SerializeAndDeserializeDateTimeJson(new DateTime(1969, 1, 1, 0, 0, 0, DateTimeKind.Local));
        }

        [Fact]
        public void JsonSerializeAndDeserialize1970Utc()
        {
            SerializeAndDeserializeDateTimeJson(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void JsonSerializeAndDeserialize1970Local()
        {
            SerializeAndDeserializeDateTimeJson(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local));
        }

        [Fact]
        public void JsonSerializeAndDeserialize1971Utc()
        {
            SerializeAndDeserializeDateTimeJson(new DateTime(1971, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void JsonSerializeAndDeserialize1971Local()
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
#endif