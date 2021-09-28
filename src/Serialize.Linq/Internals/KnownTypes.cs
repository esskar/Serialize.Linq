using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serialize.Linq.Internals
{
    internal static class KnownTypes
    {
        private static readonly IEnumerable<Type> _All = new List<Type>
        {
            typeof(bool),
            typeof(decimal), typeof(double),
            typeof(float),
            typeof(int), typeof(uint),
            typeof(short), typeof(ushort),
            typeof(long), typeof(ulong),
            typeof(string),
            typeof(DateTime), typeof(DateTimeOffset),
            typeof(TimeSpan), typeof(Guid),
            typeof(Enum),
            typeof(sbyte), typeof(byte), typeof(char)
        };

        private static readonly IDictionary<Type, AutoAddCollectionTypes> _allExploded = Explode(_All, InternalAutoAddCollectionTypes.AsBoth);

        private static readonly IDictionary<Type, AutoAddCollectionTypes> _assignables = new Dictionary<Type, AutoAddCollectionTypes>();

        public static IEnumerable<Type> GetKnown(AutoAddCollectionTypes includeCollectionTypes)
        {
            return from dlgPair in _allExploded
                   where (dlgPair.Value & includeCollectionTypes) > 0 || dlgPair.Value == AutoAddCollectionTypes.None
                   select dlgPair.Key;
        }

        public static IEnumerable<Type> GetAssignables(AutoAddCollectionTypes includeCollectionTypes)
        {
            return from dlgPair in _assignables
                   where (dlgPair.Value & includeCollectionTypes) > 0 || dlgPair.Value == AutoAddCollectionTypes.None
                   select dlgPair.Key;
        }

        public static bool Match(Type type)
        {
            return type != null && _allExploded.Concat(_assignables).Any(pair => pair.Key == type);
        }

        public static bool TryAddAsAssignable(Type type)
        {
            bool result = false;
            using (IEnumerator<KeyValuePair<Type, AutoAddCollectionTypes>> enumerator = _allExploded.GetEnumerator())
            {
                while (enumerator.MoveNext() && !result)
                {
                    if (enumerator.Current.Key.IsAssignableFrom(type))
                    {
                        _assignables.Add(type, enumerator.Current.Value);
                        result = true;
                    }
                }
            }
            return result;
        }

        public static IEnumerable<Type> Explode(IEnumerable<Type> types, AutoAddCollectionTypes includeCollectionTypes)
        {
            return Explode(types, (InternalAutoAddCollectionTypes)includeCollectionTypes).Keys;
        }

        private static IDictionary<Type, AutoAddCollectionTypes> Explode(IEnumerable<Type> types, InternalAutoAddCollectionTypes includeCollectionTypes)
        {
            var result = new Dictionary<Type, AutoAddCollectionTypes>();
            foreach (var type in types)
            {
                result.Add(type, AutoAddCollectionTypes.None);
                if (includeCollectionTypes.HasFlag(InternalAutoAddCollectionTypes.AsArray))
                    result.Add(type.MakeArrayType(), AutoAddCollectionTypes.AsArray);
                if (includeCollectionTypes.HasFlag(InternalAutoAddCollectionTypes.AsList))
                    result.Add(typeof(List<>).MakeGenericType(type), AutoAddCollectionTypes.AsList);
                if (!type.IsClass())
                {
                    var nullableType = typeof(Nullable<>).MakeGenericType(type);
                    result.Add(nullableType, AutoAddCollectionTypes.None);
                    if (includeCollectionTypes.HasFlag(InternalAutoAddCollectionTypes.AsArray))
                        result.Add(nullableType.MakeArrayType(), AutoAddCollectionTypes.AsArray);
                    if (includeCollectionTypes.HasFlag(InternalAutoAddCollectionTypes.AsList))
                        result.Add(typeof(List<>).MakeGenericType(nullableType), AutoAddCollectionTypes.AsList);
                }
            }

            return result;
        }

    }
}
