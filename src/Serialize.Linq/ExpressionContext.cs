#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serialize.Linq
{
    public class ExpressionContext : ExpressionContextBase
    {
        private readonly IAssemblyLoader _assemblyLoader;

        public ExpressionContext()
            : this(new DefaultAssemblyLoader()) { }

        public ExpressionContext(IAssemblyLoader assemblyLoader)
        {
            _assemblyLoader = assemblyLoader 
                ?? throw new ArgumentNullException(nameof(assemblyLoader));
        }

        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return _assemblyLoader.GetAssemblies();
        }
    }
}