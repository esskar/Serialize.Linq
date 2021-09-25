#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

namespace Serialize.Linq.Interfaces
{
    /// <summary>
    /// A text serializer interface.
    /// </summary>
    public interface ITextSerializer : ISerializer, IGenericSerializer<string> { }
}
