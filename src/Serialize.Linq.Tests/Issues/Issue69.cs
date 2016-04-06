using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Extensions;
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
            this.SerialzeAndDeserializeDateTimeJson(new DateTime(1969, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [TestMethod]
        public void JsonSerialzeAndDeserialize1969Local()
        {
            this.SerialzeAndDeserializeDateTimeJson(new DateTime(1969, 1, 1, 0, 0, 0, DateTimeKind.Local));
        }

        [TestMethod]
        public void JsonSerialzeAndDeserialize1970Utc()
        {
            this.SerialzeAndDeserializeDateTimeJson(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));            
        }

        [TestMethod]
        public void JsonSerialzeAndDeserialize1970Local()
        {
            this.SerialzeAndDeserializeDateTimeJson(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local));
        }

        [TestMethod]
        public void JsonSerialzeAndDeserialize1971Utc()
        {
            this.SerialzeAndDeserializeDateTimeJson(new DateTime(1971, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [TestMethod]
        public void JsonSerialzeAndDeserialize1971Local()
        {
            this.SerialzeAndDeserializeDateTimeJson(new DateTime(1971, 1, 1, 0, 0, 0, DateTimeKind.Local));
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