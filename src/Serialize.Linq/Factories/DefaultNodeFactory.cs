#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Factories
{
    public class DefaultNodeFactory : TypeResolverNodeFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNodeFactory"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="factorySettings">The factory settings to use.</param>
        public DefaultNodeFactory(Type type, FactorySettings factorySettings = null)
            : this(new List<Type> { type }, factorySettings) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNodeFactory"/> class.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="factorySettings">The factory settings to use.</param>
        /// <exception cref="System.ArgumentNullException">types</exception>
        /// <exception cref="System.ArgumentException">types</exception>
        public DefaultNodeFactory(IEnumerable<Type> types, FactorySettings factorySettings = null)
            : base(GetExpectedTypes(types), factorySettings) { }

        /// <summary>
        /// Collects the types for <see cref="TypeResolverNodeFactory"/>.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Type> GetExpectedTypes(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));
            var expectedTypes = new HashSet<Type>();
            foreach (var type in types)
            {
                if (type == null)
                    throw new ArgumentException("All types must be non-null.", nameof(types));
                expectedTypes.UnionWith(ComplexPropertyMemberTypeFinder.FindTypes(type));
            }
            return expectedTypes;
        }
    }
}
