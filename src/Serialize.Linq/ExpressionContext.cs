#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Reflection;
using System.Collections.Generic;

namespace Serialize.Linq
{
    public class ExpressionContext : ExpressionContextBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
#if NETSTANDARD1_3
            return System.Linq.Enumerable.Empty<Assembly>();
#elif (NETCOREAPP1_0 || NETCOREAPP1_1 || UAP10_0)
            return System.Linq.Enumerable.Empty<Assembly>();
            // return System.AppDomain.NetCoreApp.AppDomain.CurrentDomain.GetAssemblies(GetType());
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }
    }
}