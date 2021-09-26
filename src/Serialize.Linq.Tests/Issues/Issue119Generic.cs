using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    // https://github.com/esskar/Serialize.Linq/issues/119
    [TestClass]
    public class Issue119Generic
    {
        private JsonSerializer _expressionSerializer;
        private Expression<Func<IDictionary<string, string>, bool>> _filterExpression;

        [TestInitialize]
        public void Initialize()
        {
            Expression<Func<MyEntity, bool>> originalExpression = x => x.Age < 10;
            var visitor = new ReplaceToDictionaryVisitor(typeof(MyEntity).Name);
            var newExpression = (Expression<Func<IDictionary<string, string>, bool>>)visitor.Visit(originalExpression);

            _expressionSerializer = new JsonSerializer();

            _filterExpression = newExpression;
        }

        [TestMethod]
        public void ModifiedExpressionShouldWork()
        {
            ModifiedExpressionShouldWork(_filterExpression);
        }

        private static void ModifiedExpressionShouldWork(Expression<Func<IDictionary<string, string>, bool>> expression)
        {
            var items = new Dictionary<string, string> { { "Age", "9" } };
            var result1 = expression.Compile()(items);
            Assert.IsTrue(result1);

            items = new Dictionary<string, string> { { "Age", "10" } };
            var result2 = expression.Compile()(items);
            Assert.IsFalse(result2);
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
            var actual = Deserialize<IDictionary<string, string>>(text);
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void DeserializeIndexExpressionShouldWork()
        {
            var text = Serialize(_filterExpression);
            var actual = Deserialize<IDictionary<string, string>>(text);
            ModifiedExpressionShouldWork(actual);
        }

        private string Serialize<T>(Expression<Func<T, bool>> predicate)
        {
            var text = _expressionSerializer.SerializeGeneric(predicate);
            return text;
        }

        private Expression<Func<T, bool>> Deserialize<T>(string text)
        {
            var expression = _expressionSerializer.DeserializeGeneric(text);
            return (Expression<Func<T, bool>>)expression;
        }

        public class MyEntity
        {
            public int Age { get; set; }
        }

        public class ReplaceToDictionaryVisitor : ExpressionVisitor
        {
            private readonly string _messageTypeName;
            private static readonly PropertyInfo _itemProperty = typeof(IDictionary<string, string>).GetProperty("Item");
            private readonly ParameterExpression _targetParameter = Expression.Parameter(typeof(IDictionary<string, string>));

            public ReplaceToDictionaryVisitor(string messageTypeName)
            {
                _messageTypeName = messageTypeName;
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                var body = this.Visit(node.Body);
                Debug.Assert(body != null, nameof(body) + " != null");

                var parameters = node.Parameters.Select(this.Visit).Cast<ParameterExpression>();
                return Expression.Lambda(body, parameters);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node.Type.Name.Equals(_messageTypeName) ? this._targetParameter : node;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member.MemberType != MemberTypes.Property)
                {
                    throw new NotSupportedException();
                }

                var memberName = node.Member.Name;
                var dicValue = Expression.Property(
                    this.Visit(node.Expression),
                    _itemProperty,
                    Expression.Constant(memberName)
                );

                if (node.Type == typeof(string))
                    return Expression.Convert(dicValue, ((PropertyInfo)node.Member).PropertyType);

                var changeTypeMethod = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });
                Debug.Assert(changeTypeMethod != null, nameof(changeTypeMethod) + " != null");

                var callExpressionReturningObject = Expression.Call(changeTypeMethod, dicValue, Expression.Constant(node.Type));
                return Expression.Convert(callExpressionReturningObject, node.Type);
            }
        }

    }
}