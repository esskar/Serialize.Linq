#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Linq.Expressions;
using System.Reflection;
using Serialize.Linq.Factories;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Interfaces
{
    public interface INodeFactory
    {
        /// <summary>
        /// Returns the factory settings for this instance
        /// </summary>
        FactorySettings Settings { get; }

        /// <summary>
        /// Creates the specified expression node an expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        ExpressionNode Create(Expression expression);

        /// <summary>
        /// Creates the specified type node from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        TypeNode Create(Type type);

        /// <summary>
        /// Gets binding flags to be used when accessing type members.
        /// </summary>
        BindingFlags BindingFlags { get; }

        /// <summary>
        /// Gets binding flags to be used when accessing type members.
        /// </summary>
        [Obsolete("Use INodeFactory.Binding instead.", false)]
        BindingFlags? GetBindingFlags();
    }    
}