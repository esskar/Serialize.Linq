using System.Linq;

namespace Serialize.Linq.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsEqualToAny<T>(this T item, params T[] items)
        {
            return items.Contains(item);
        }
    }
}
