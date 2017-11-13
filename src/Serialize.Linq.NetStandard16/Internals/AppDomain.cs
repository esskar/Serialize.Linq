using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Serialize.Linq.Internals
{
    internal class AppDomain
    {
        public static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        private Assembly[] _assemblies;

        public Assembly[] GetAssemblies()
        {
            if (_assemblies == null)
            {
                var assemblies = new List<Assembly>();
                var dependencies = DependencyContext.Default.RuntimeLibraries;
                foreach (var library in dependencies)
                {
                    if (IsCandidateCompilationLibrary(library))
                    {
                        try
                        {
                            var assembly = Assembly.Load(new AssemblyName(library.Name));
                            assemblies.Add(assembly);
                        }
                        catch(FileNotFoundException)
                        {
                            // ignore
                        }
                    }
                }
                _assemblies = assemblies.ToArray();
            }
            return _assemblies;
        }

        private static bool IsCandidateCompilationLibrary(RuntimeLibrary compilationLibrary)
        {
            return IsCandidateCompilationLibrary(compilationLibrary, "Serialize.Linq")
                || IsCandidateCompilationLibrary(compilationLibrary, "System");
        }

        private static bool IsCandidateCompilationLibrary(RuntimeLibrary compilationLibrary, string assemblyName)
        {
            return compilationLibrary.Name.StartsWith(assemblyName)
                || compilationLibrary.Dependencies.Any(d => d.Name.StartsWith(assemblyName));
        }
    }
}
