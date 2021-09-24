using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Serialize.Linq.Internals
{
    internal static class KnownTypes
    {
        private static readonly Type[] _All =
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
            typeof(sbyte), typeof(byte), typeof(byte)
        };

        private static readonly IDictionary<Type, AutoAddCollectionTypes> _allExploded = Explode(_All, InternalAutoAddCollectionTypes.AsBoth);

        private static readonly IDictionary<Type, AutoAddCollectionTypes> _assignables = new Dictionary<Type, AutoAddCollectionTypes>();

        [Obsolete("Use KnownTypes.GetKnown or KnownTypes.GetAssignables instead.", false)]
        public static readonly Type[] All = _All;

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
            bool result;

            result = type != null && _allExploded.Concat(_assignables).Any(pair => pair.Key == type);
            if (!result)
            {
                using (IEnumerator<KeyValuePair<Type, AutoAddCollectionTypes>> tmpEnumerator = _allExploded.GetEnumerator())
                {
                    while (tmpEnumerator.MoveNext() && !result)
                    {
                        if (tmpEnumerator.Current.Key.IsAssignableFrom(type))
                        {
                            _assignables.Add(type, tmpEnumerator.Current.Value);
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        [Obsolete("Use Explode(IEnumerable<Type>, AutomaticAddKnownCollections) instead.", false)]
        public static IEnumerable<Type> Explode(IEnumerable<Type> types, bool includeArrayTypes, bool includeListTypes)
        {
            if (includeArrayTypes && includeListTypes)
            {
                return Explode(types, AutoAddCollectionTypes.AsArray | AutoAddCollectionTypes.AsList);
            }
            else if (includeArrayTypes)
            {
                return Explode(types, AutoAddCollectionTypes.AsArray);
            }
            else if (includeListTypes)
            {
                return Explode(types, AutoAddCollectionTypes.AsList);
            }
            else
            {
                return Explode(types, AutoAddCollectionTypes.None);
            }
        }

        public static IEnumerable<Type> Explode(IEnumerable<Type> valTypes, AutoAddCollectionTypes includeCollectionTypes)
        {
            return Explode(valTypes, (InternalAutoAddCollectionTypes)includeCollectionTypes).Keys;
        }

        private static IDictionary<Type, AutoAddCollectionTypes> Explode(IEnumerable<Type> vtypes, InternalAutoAddCollectionTypes includeCollectionTypes)
        {
            Type nullableType;
            IDictionary<Type, AutoAddCollectionTypes> tempTypes;

            tempTypes = new Dictionary<Type, AutoAddCollectionTypes>();
            foreach (Type tmpType in vtypes)
            {
                tempTypes.Add(tmpType, AutoAddCollectionTypes.None);
                if (includeCollectionTypes.HasFlag(InternalAutoAddCollectionTypes.AsArray))
                    tempTypes.Add(tmpType.MakeArrayType(), AutoAddCollectionTypes.AsArray);
                if (includeCollectionTypes.HasFlag(InternalAutoAddCollectionTypes.AsList))
                    tempTypes.Add(typeof(List<>).MakeGenericType(tmpType), AutoAddCollectionTypes.AsList);
                if (!tmpType.IsClass())
                {
                    nullableType = typeof(Nullable<>).MakeGenericType(tmpType);
                    tempTypes.Add(nullableType, AutoAddCollectionTypes.None);
                    if (includeCollectionTypes.HasFlag(InternalAutoAddCollectionTypes.AsArray))
                        tempTypes.Add(nullableType.MakeArrayType(), AutoAddCollectionTypes.AsArray);
                    if (includeCollectionTypes.HasFlag(InternalAutoAddCollectionTypes.AsList))
                        tempTypes.Add(typeof(List<>).MakeGenericType(nullableType), AutoAddCollectionTypes.AsList);
                }
            }

            return tempTypes;
        }

    }
}
