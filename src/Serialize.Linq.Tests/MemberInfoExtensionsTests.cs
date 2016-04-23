#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System.Reflection;
using Xunit;
using Serialize.Linq.Extensions;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests
{
    
    public class MemberInfoExtensionsTests
    {
        

        [Fact]
        public void GetReturnTypeOfPropertyTest()
        {
            var actual = typeof(Bar).GetProperty("FirstName").GetReturnType();
            Assert.Equal(typeof(string), actual);
        }

        [Fact]
        public void GetReturnTypeOfFieldTest()
        {
            var actual = typeof(Bar).GetField("IsFoo").GetReturnType();
            Assert.Equal(typeof(bool), actual);
        }

        [Fact]
        public void GetReturnTypeOfMethodTest()
        {
            var actual = typeof(Bar).GetMethod("GetName").GetReturnType();
            Assert.Equal(typeof(string), actual);
        }
    }
}