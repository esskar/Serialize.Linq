using System;
using System.Collections.Generic;
using System.Reflection;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Internals
{
    internal class DefaultAssemblyLoader : IAssemblyLoader
    {
        public IEnumerable<Assembly> GetAssemblies()
        {
#if NETSTANDARD1_3 || NETCOREAPP1_0 || NETCOREAPP1_1 || UAP10_0
            throw new NotSupportedException(
                "Please provide a custom implemention for the IAssemblyLoader, with `ExpressionExtensions.AssemblyLoader = new MyCustomLoader();`, to retrieve assemblies that have been loaded into the execution context of this application domain.\r\n" +
                "You could use the NuGet package 'System.AppDomain.NetCoreApp' which mimics the AppDomain."
            );
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }
    }
}