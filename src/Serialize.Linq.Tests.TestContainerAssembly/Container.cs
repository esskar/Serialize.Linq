using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Serialize.Linq.Tests.TestDesiredAssembly
{
    public class Container
    {
        public Expression GetExpression()
        {
            Expression<Func<IEnumerable<int>, IEnumerable<int>>> Expr = c =>
                from x in c
                let someConst_6547588C372F49698EC3B242C745FCA0 = 8
                where (x == someConst_6547588C372F49698EC3B242C745FCA0)
                select x;

            return Expr;
        }
    }
}
