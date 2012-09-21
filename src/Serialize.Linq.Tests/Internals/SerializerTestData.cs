using System;
using System.Linq.Expressions;

namespace Serialize.Linq.Tests.Internals
{
    internal static class SerializerTestData
    {
        public static readonly Expression[] TestExpressions = new Expression[]  {
            null,
            (Expression<Func<bool, bool>>)(b => b), 
            (Expression<Func<Bar, bool>>)(p => p.LastName == "Miller" && p.FirstName.StartsWith("M")),          
        };
    }
}