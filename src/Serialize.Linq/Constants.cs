using System.Reflection;

namespace Serialize.Linq
{
    internal static class Constants
    {
        public const BindingFlags ALSO_NON_PUBLIC_BINDING = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

        public const BindingFlags PUBLIC_ONLY_BINDING = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
    }
}
