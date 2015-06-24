#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Interfaces
{
    public interface ISerializer
    {
        /// <summary>
        /// If set to true, and types are added as known types, also adds the type as array
        /// Example: string -> string[] 
        /// </summary>
        /// <remarks>
        /// If set to true, <see cref="AutoAddKnownTypesAsListTypes"/> will be set to false.
        /// </remarks>
        bool AutoAddKnownTypesAsArrayTypes { get; set; }

        /// <summary>
        /// If set to true, and types are added as known types, also adds the type as list
        /// Example: string -> List&lt;string&gt;
        /// </summary>
        /// <remarks>
        /// If set to true, <see cref="AutoAddKnownTypesAsArrayTypes"/> will be set to false.
        /// </remarks>
        bool AutoAddKnownTypesAsListTypes { get; set; }

        /// <summary>
        /// Adds a new type to the list of known types.
        /// </summary>
        /// <param name="type">The type.</param>
        void AddKnownType(Type type);

        /// <summary>
        /// Adds a collection of new types to the list of known types.
        /// </summary>
        /// <param name="types">The types.</param>
        void AddKnownTypes(IEnumerable<Type> types);

        /// <summary>
        /// Serializes the specified object to the specified stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="obj">The obj.</param>
        void Serialize<T>(Stream stream, T obj) where T : Node;

        /// <summary>
        /// Deserializes an object of type T from the specified stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        T Deserialize<T>(Stream stream) where T : Node;
    }
}
