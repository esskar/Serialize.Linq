#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System.Reflection;
using System.Collections.Generic;

namespace Serialize.Linq
{
    public class ExpressionContext : ExpressionContextBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
#if NETSTANDARD1_3 || UAP10_0
            return Extensions.ExpressionExtensions.AssemblyLoader.GetAssemblies();
#else
            return System.AppDomain.CurrentDomain.GetAssemblies();
#endif
        }
    }
}