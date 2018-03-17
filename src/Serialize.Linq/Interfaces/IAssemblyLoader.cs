using System.Collections.Generic;
using System.Reflection;

namespace Serialize.Linq.Interfaces
{
    /// <summary>
    /// IAssemblyLoader interface which is used to retrieve assemblies that have been loaded into the execution context of this application domain.
    /// 
    /// By default this interface will be implemented by the DefaultAssemblyLoader.
    /// </summary>
    public interface IAssemblyLoader
    {
        /// <summary>
        /// Gets the assemblies that have been loaded into the execution context of this application domain.
        /// </summary>
        /// 
        /// <returns>
        /// An array of assemblies in this application domain.
        /// </returns>
        IEnumerable<Assembly> GetAssemblies();
    }
}