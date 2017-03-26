using Serialize.Linq.Internals;
using System.Collections.Generic;
using System.Reflection;

namespace Serialize.Linq
{
    public class ExpressionContext : ExpressionContextBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}