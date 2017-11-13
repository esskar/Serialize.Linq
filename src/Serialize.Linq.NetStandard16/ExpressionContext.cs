using Serialize.Linq.Internals;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Serialize.Linq
{
    public class ExpressionContext : ExpressionContextBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
#if !NETCOREAPP2_0 && !NETCOREAPP1_1 && !NETSTANDARD1_6
            return AppDomain.CurrentDomain.GetAssemblies();
#else
            var depContext = DependencyContext.Default;
            var runtimeAssemblyGroups = depContext.RuntimeLibraries.Select(r => r.RuntimeAssemblyGroups).ToList();
            var assetPaths = runtimeAssemblyGroups.SelectMany(rs => (string.IsNullOrEmpty(depContext.Target.Runtime) 
                    ? rs?.GetDefaultAssets()
                    : rs?.GetRuntimeAssets(depContext.Target.Runtime)) ?? Enumerable.Empty<string>()); 

            var assemblies = new List<Assembly>();
            foreach (var assetPath in assetPaths)
            {
                var assemblyName = Path.GetFileNameWithoutExtension(assetPath);
                var assemblyNameR = new AssemblyName(assemblyName);
                var assembly = Assembly.Load(assemblyNameR);
                assemblies.Add(assembly);
            }

            return assemblies;
#endif
        }
    }
}