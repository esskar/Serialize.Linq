#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Serialize.Linq.Tests.Internals
{
    internal static class SerializerTestData
    {
        private static readonly int[] _arrayOfIds = { 6, 7, 8, 9, 10 };
        private static readonly List<int> _listOfIds = new List<int> { 6, 7, 8, 9, 10 };
        public static readonly List<Expression> TestExpressions = new List<Expression>
        {
            null,
            (Expression<Func<bool, bool>>)(b => b), 
            (Expression<Func<Bar, bool>>)(p => p.LastName == "Miller" && p.FirstName.StartsWith("M")),        
            (Expression<Func<Bar, bool>>)(p => (new [] { 1, 2, 3, 4, 5 }).Contains(p.Id)),
            (Expression<Func<bool>>)(() => true),
            (Expression<Func<bool>>)(() => false),
            (Expression<Func<bool>>)(() => 5 != 4),
            (Expression<Func<int>>)(() => 42),
            (Expression<Func<Guid>>)(() => Guid.NewGuid()),
            (Expression<Func<Guid>>)(() => new Guid("00000000-0000-0000-0000-00000000000")),
            (Expression<Func<Guid>>)(() => Guid.Empty),
            (Expression<Func<DayOfWeek, bool>>)(p => p == DayOfWeek.Monday),
            (Expression<Func<Struct>>)(() => new Struct()),
            (Expression<Func<Struct, string>>)(s => s.Name),
            (Expression<Func<EmptyStruct>>)(() => new EmptyStruct()),
            (Expression<Func<Bar>>)(() => new Bar()),
            (Expression<Func<IEnumerable<int>, IEnumerable<int>>>)(c => from x in c let a = 100 where (x == a) select x)
			
        };
        public static readonly Expression[] TestNodesOnlyExpressions =
        {
            (Expression<Func<Bar, bool>>)(p => _arrayOfIds.Contains(p.Id)),
            (Expression<Func<Bar, bool>>)(p => _listOfIds.Contains(p.Id))
        };

        static SerializerTestData()
        {
            AddLetExpressions();
        }

        private static void AddLetExpressions()
        {
            Expression<Func<IEnumerable<int>, IEnumerable<int>>> intExpr = c => 
                from x in c
                let test = 8
                where x == test
                select x;
            TestExpressions.Add(intExpr);

            Expression<Func<IEnumerable<string>, IEnumerable<string>>> strExpr = c => 
                from x in c
                let test = "bar"
                where x == test
                select x;
            TestExpressions.Add(strExpr);
        }
    }
}