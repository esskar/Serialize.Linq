using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests.Issues
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/69
    /// </summary>
    [TestClass]
    public class Issue69
    {
        private ExpressionSerializer _jsonExpressionSerializer;

        [TestInitialize]
        public void Initialize()
        {
            _jsonExpressionSerializer = new ExpressionSerializer(new JsonSerializer());
        }

        [TestMethod]
        public void JsonSerialzeAndDeserialize1969Utc()
        {
            SerialzeAndDeserializeDateTimeJson(new DateTime(1969, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [TestMethod]
        public void JsonSerialzeAndDeserialize1969Local()
        {
            SerialzeAndDeserializeDateTimeJson(new DateTime(1969, 1, 1, 0, 0, 0, DateTimeKind.Local));
        }

        [TestMethod]
        public void JsonSerialzeAndDeserialize1970Utc()
        {
            SerialzeAndDeserializeDateTimeJson(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));            
        }

        [TestMethod]
        public void JsonSerialzeAndDeserialize1970Local()
        {
            SerialzeAndDeserializeDateTimeJson(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local));
        }

        [TestMethod]
        public void JsonSerialzeAndDeserialize1971Utc()
        {
            SerialzeAndDeserializeDateTimeJson(new DateTime(1971, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [TestMethod]
        public void JsonSerialzeAndDeserialize1971Local()
        {
            SerialzeAndDeserializeDateTimeJson(new DateTime(1971, 1, 1, 0, 0, 0, DateTimeKind.Local));
        }
        
        private void SerialzeAndDeserializeDateTimeJson(DateTime dt)
        {
            Expression<Func<DateTime>> actual = () => dt;
            actual = actual.Update(Expression.Constant(dt), new List<ParameterExpression>());

            var serialized = _jsonExpressionSerializer.SerializeText(actual);
            var expected = _jsonExpressionSerializer.DeserializeText(serialized);
            ExpressionAssert.AreEqual(expected, actual);
        }
    }
}