using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serialize.Linq.Tests.Internals
{
    internal static class ExpressionAssert
    {
        public static void AreEqual<TDelegate>(Expression<TDelegate> expected, Expression<TDelegate> actual, string message = null)
        {
            AreEqual(expected, (Expression)actual, message);
        }

        public static void AreEqual(Expression expected, Expression actual, string message = null)
        {
            var comparer = new ExpressionComparer();
            var result = comparer.AreEqual(expected, actual);
            if (result) 
                return;

            var failMessage = !string.IsNullOrWhiteSpace(message) ? message : string.Empty;
            failMessage += $"Expected was <{expected}>, Actual was <{actual}>";
            Assert.Fail(failMessage);
        }
    }
}