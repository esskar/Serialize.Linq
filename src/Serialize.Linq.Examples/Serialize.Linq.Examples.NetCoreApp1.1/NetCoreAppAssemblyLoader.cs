using System;
using System.AppDomain.NetCoreApp;
using System.Collections.Generic;
using System.Reflection;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Examples.NetCoreApp1
{
    internal class NetCoreAppAssemblyLoader : IAssemblyLoader
    {
        private static readonly object Locked = new object();
        private static IEnumerable<Assembly> _assemblies;

        public IEnumerable<Assembly> GetAssemblies()
        {
            lock (Locked)
            {
                if (_assemblies == null)
                {
                    Type type = GetType();
                    _assemblies = AppDomain.CurrentDomain.GetAssemblies(type);
                }

                return _assemblies;
            }
        }
    }
}