using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Serialize.Linq.Tests.Internals
{
    internal static class TestContextExtensions
    {
        public static void WriteLine(this TestContext testContext, string fmt, params object[] args)
        {
            Console.WriteLine(fmt, args);
        }
    }
}
