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

        private static readonly IDictionary<Type, AutomaticAddKnownCollections> _allExploded = Explode(_All, InternalAutomaticAddKnownCollections.AsBoth);

        private static readonly IDictionary<Type, AutomaticAddKnownCollections> _assignables = new Dictionary<Type, AutomaticAddKnownCollections>();

        [Obsolete("Use KnownTypes.GetKnown or KnownTypes.GetAssignables instead.", false)]
        public static readonly Type[] All = _All;

        public static IEnumerable<Type> GetKnown(AutomaticAddKnownCollections valIncludeCollectionTypes)
        {
            return from dlgPair in _allExploded
                   where (dlgPair.Value & valIncludeCollectionTypes) > 0 || dlgPair.Value == AutomaticAddKnownCollections.None
                   select dlgPair.Key;
        }

        public static IEnumerable<Type> GetAssignables(AutomaticAddKnownCollections valIncludeCollectionTypes)
        {
            return from dlgPair in _assignables
                   where (dlgPair.Value & valIncludeCollectionTypes) > 0 || dlgPair.Value == AutomaticAddKnownCollections.None
                   select dlgPair.Key;
        }

        public static bool Match(Type type)
        {
            bool result;

            result = type != null && _allExploded.Concat(_assignables).Any(pair => pair.Key == type);
            if (!result)
            {
                using (IEnumerator<KeyValuePair<Type, AutomaticAddKnownCollections>> tmpEnumerator = _allExploded.GetEnumerator())
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
                return Explode(types, AutomaticAddKnownCollections.AsArray | AutomaticAddKnownCollections.AsList);
            }
            else if (includeArrayTypes)
            {
                return Explode(types, AutomaticAddKnownCollections.AsArray);
            }
            else if (includeListTypes)
            {
                return Explode(types, AutomaticAddKnownCollections.AsList);
            }
            else
            {
                return Explode(types, AutomaticAddKnownCollections.None);
            }
        }

        public static IEnumerable<Type> Explode(IEnumerable<Type> valTypes, AutomaticAddKnownCollections valIncludeCollectionTypes)
        {
            return Explode(valTypes, (InternalAutomaticAddKnownCollections)valIncludeCollectionTypes).Keys;
        }

        private static IDictionary<Type, AutomaticAddKnownCollections> Explode(IEnumerable<Type> valTypes, InternalAutomaticAddKnownCollections valIncludeCollectionTypes)
        {
            Type nullableType;
            IDictionary<Type, AutomaticAddKnownCollections> types;

            types = new Dictionary<Type, AutomaticAddKnownCollections>();
            foreach (Type tmpType in valTypes)
            {
                types.Add(tmpType, AutomaticAddKnownCollections.None);
                if (valIncludeCollectionTypes.HasFlag(InternalAutomaticAddKnownCollections.AsArray))
                    types.Add(tmpType.MakeArrayType(), AutomaticAddKnownCollections.AsArray);
                if (valIncludeCollectionTypes.HasFlag(InternalAutomaticAddKnownCollections.AsList))
                    types.Add(typeof(List<>).MakeGenericType(tmpType), AutomaticAddKnownCollections.AsList);
                if (!tmpType.IsClass())
                {
                    nullableType = typeof(Nullable<>).MakeGenericType(tmpType);
                    types.Add(nullableType, AutomaticAddKnownCollections.None);
                    if (valIncludeCollectionTypes.HasFlag(InternalAutomaticAddKnownCollections.AsArray))
                        types.Add(nullableType.MakeArrayType(), AutomaticAddKnownCollections.AsArray);
                    if (valIncludeCollectionTypes.HasFlag(InternalAutomaticAddKnownCollections.AsList))
                        types.Add(typeof(List<>).MakeGenericType(nullableType), AutomaticAddKnownCollections.AsList);
                }
            }

            return types;
        }

    }
}
