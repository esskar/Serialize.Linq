using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Serialize.Linq.Tests.Internals
{
    internal static class SerializerTestData
    {
        private static readonly int[] __arrayOfIds = new []{ 6, 7, 8, 9, 10 };
        private static readonly List<int> __listOfIds = new List<int>{ 6, 7, 8, 9, 10 };        
        public static readonly Expression[] TestExpressions = new Expression[]  {
            null,
            (Expression<Func<bool, bool>>)(b => b), 
            (Expression<Func<Bar, bool>>)(p => p.LastName == "Miller" && p.FirstName.StartsWith("M")),        
            (Expression<Func<Bar, bool>>)(p => (new [] { 1, 2, 3, 4, 5 }).Contains(p.Id)),
            (Expression<Func<bool>>)(() => true),
            (Expression<Func<bool>>)(() => false),
            (Expression<Func<bool>>)(() => 5 != 4),
            (Expression<Func<int>>)(() => 42),
        };
        public static readonly Expression[] TestNodesOnlyExpressions = new Expression[]  {
            (Expression<Func<Bar, bool>>)(p => __arrayOfIds.Contains(p.Id)),
            (Expression<Func<Bar, bool>>)(p => __listOfIds.Contains(p.Id))
        };
    }
}