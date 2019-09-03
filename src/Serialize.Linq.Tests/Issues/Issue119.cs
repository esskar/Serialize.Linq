using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    // https://github.com/esskar/Serialize.Linq/issues/119
    [TestClass]
    public class Issue119
    {
        private ExpressionSerializer _expressionSerializer;
        private Expression<Func<Dictionary<string, string>, bool>> _filterExpression;

        [TestInitialize]
        public void Initialize()
        {
            _expressionSerializer = new ExpressionSerializer(new JsonSerializer());
            _filterExpression = filter
                => (int)Convert.ChangeType(filter["Age"], typeof(int)) < 10;
        }

        [TestMethod]
        public void SerializeIndexExpression()
        {
            var text = Serialize(_filterExpression);
            Assert.IsNotNull(text);
        }

        [TestMethod]
        public void DeserializeIndexExpression()
        {
            var text = Serialize(_filterExpression);
            var actual = Deserialize<Dictionary<string, string>>(text);
            Assert.IsNotNull(actual);
        }

        private string Serialize<T>(Expression<Func<T, bool>> predicate)
        {
            var text = _expressionSerializer.SerializeText(predicate);
            return text;
        }

        private Expression<Func<T, bool>> Deserialize<T>(string text)
        {
            var expression = _expressionSerializer.DeserializeText(text);
            return (Expression<Func<T, bool>>) expression;
        }
    }
}
