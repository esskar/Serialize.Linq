using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Serialize.Linq.Tests.Container
{
    public class ExpressionContainer
    {
        public Expression GetExpression()
        {
            Expression<Func<IEnumerable<int>, IEnumerable<int>>> expr = c =>
                from x in c
                let someConst6547588C372F49698Ec3B242C745Fca0 = 8
                where (x == someConst6547588C372F49698Ec3B242C745Fca0)
                select x;

            return expr;
        }
    }
}
