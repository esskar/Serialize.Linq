using System;
using System.Collections.Generic;
using System.Linq;

#if WINDOWS_PHONE7
using Serialize.Linq.Internals;
#endif

namespace Serialize.Linq.Serializers
{
    public abstract class SerializerBase
    {
        private static readonly Type[] _knownTypes =
        { 
            typeof(bool),
            typeof(decimal), typeof(double),
            typeof(float),
            typeof(int), typeof(uint),
            typeof(short), typeof(ushort),
            typeof(long), typeof(ulong),
            typeof(string),
            typeof(DateTime), typeof(TimeSpan), typeof(Guid), typeof(DayOfWeek)
        };

        private readonly HashSet<Type> _customKnownTypes;
        private bool _autoAddKnownTypesAsArrayTypes;
        private bool _autoAddKnownTypesAsListTypes;

        protected SerializerBase()
        {
            _customKnownTypes = new HashSet<Type>();
            this.AutoAddKnownTypesAsArrayTypes = true;
        }

        public bool AutoAddKnownTypesAsArrayTypes
        {
            get { return _autoAddKnownTypesAsArrayTypes; }
            set
            {
                _autoAddKnownTypesAsArrayTypes = value;
                if (value)
                    _autoAddKnownTypesAsListTypes = false;
            }
        }

        public bool AutoAddKnownTypesAsListTypes
        {
            get { return _autoAddKnownTypesAsListTypes; }
            set
            {
                _autoAddKnownTypesAsListTypes = value;
                if (value)
                    _autoAddKnownTypesAsArrayTypes = false;
            }
        }

        public void AddKnownType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            _customKnownTypes.Add(type);
        }

        public void AddKnownTypes(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException("types");

            foreach (var type in types)
                this.AddKnownType(type);
        }

        protected virtual IEnumerable<Type> GetKnownTypes()
        {
            return this.ExplodeKnownTypes(_knownTypes).Concat(this.ExplodeKnownTypes(_customKnownTypes));
        }

        private IEnumerable<Type> ExplodeKnownTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                yield return type;
                if (this.AutoAddKnownTypesAsArrayTypes)
                    yield return type.MakeArrayType();
                else if (this.AutoAddKnownTypesAsListTypes)
                    yield return typeof(List<>).MakeGenericType(type);
                
                if (type.IsClass) 
                    continue;

                var nullableType = typeof (Nullable<>).MakeGenericType(type);
                yield return nullableType;
                if (this.AutoAddKnownTypesAsArrayTypes)
                    yield return nullableType.MakeArrayType();
                else if (this.AutoAddKnownTypesAsListTypes)
                    yield return typeof(List<>).MakeGenericType(nullableType);
            }
        }
    }
}
