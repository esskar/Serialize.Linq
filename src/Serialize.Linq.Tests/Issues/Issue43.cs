using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using Serialize.Linq.Extensions;

namespace Serialize.Linq.Tests.Issues
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/43
    /// </summary>

    public class Issue43
    {
        class User
        {
            public Guid Id
            {
                get;
                set;
            }
        }

        [Fact(Skip = "ThrowsStackOverflowException")]
        public void ThrowsStackOverflowException()
        {
            var idCollection = new List<Guid>();
            for (var i = 0; i < 15000; i++)
            {
                idCollection.Add(Guid.NewGuid());
            }

            var propertyInfo = typeof(User).GetProperty("Id");
            var userParam = Expression.Parameter(typeof(User), "x");
            var propertyExpression = Expression.Property(userParam, propertyInfo);
            Expression composedExpression = Expression.Constant(false, typeof(bool));

            composedExpression = idCollection.Select(t => Expression.MakeBinary(ExpressionType.Equal, propertyExpression, Expression.Constant(t))).Aggregate<Expression, Expression>(composedExpression, (current, equalExpression) => Expression.MakeBinary(ExpressionType.OrElse, equalExpression, current));
            var expressionNode = composedExpression.ToExpressionNode();
        }
    }
}
