using System;
using System.Collections.Generic;
using System.Linq;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Serializers
{
    public abstract class SerializerBase
    {
        private readonly HashSet<Type> _customKnownTypes;
        private bool _autoAddKnownTypesAsArrayTypes;
        private bool _autoAddKnownTypesAsListTypes;
        private AutomaticAddKnownCollections _autoAddKnownTypesCollectionType;
        private IEnumerable<Type> _knownTypesExploded;

        protected SerializerBase()
        {
            _customKnownTypes = new HashSet<Type>();
            AutoAddKnownTypesCollectionType = AutomaticAddKnownCollections.AsArray;
        }

        [Obsolete("Use SerializerBase.AutoAddKnownTypesCollectionType", false)]
        public bool AutoAddKnownTypesAsArrayTypes
        {
            get => _autoAddKnownTypesAsArrayTypes;
            set
            {
                _autoAddKnownTypesAsArrayTypes = value;
                if (value)
                {
                    _autoAddKnownTypesAsListTypes = false;
                    _autoAddKnownTypesCollectionType = AutomaticAddKnownCollections.AsArray;
                }
                else
                {
                    _autoAddKnownTypesCollectionType = AutomaticAddKnownCollections.None;
                }
                _knownTypesExploded = null;
            }
        }

        [Obsolete("Use SerializerBase.AutoAddKnownTypesCollectionType", false)]
        public bool AutoAddKnownTypesAsListTypes
        {
            get => _autoAddKnownTypesAsListTypes;
            set
            {
                _autoAddKnownTypesAsListTypes = value;
                if (value)
                {
                    _autoAddKnownTypesAsArrayTypes = false;
                    _autoAddKnownTypesCollectionType = AutomaticAddKnownCollections.AsList;
                }
                else
                {
                    _autoAddKnownTypesCollectionType = AutomaticAddKnownCollections.None;
                }
                _knownTypesExploded = null;
            }
        }

        public AutomaticAddKnownCollections AutoAddKnownTypesCollectionType
        {
            get => _autoAddKnownTypesCollectionType;
            set
            {
                _autoAddKnownTypesCollectionType = value;
                _autoAddKnownTypesAsArrayTypes = _autoAddKnownTypesCollectionType == AutomaticAddKnownCollections.AsArray;
                _autoAddKnownTypesAsListTypes = _autoAddKnownTypesCollectionType == AutomaticAddKnownCollections.AsList;
            }
        }

        public void AddKnownType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            _customKnownTypes.Add(type);
            _knownTypesExploded = null;
        }

        public void AddKnownTypes(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            foreach (var type in types)
                AddKnownType(type);
        }

        protected virtual IEnumerable<Type> GetKnownTypes()
        {
            if (_knownTypesExploded == null)
            {
                _knownTypesExploded = KnownTypes.GetKnown(_autoAddKnownTypesCollectionType)
                    .Union(KnownTypes.GetAssignables(_autoAddKnownTypesCollectionType))
                    .Union(ExplodeKnownTypes(_customKnownTypes));
            }
            return _knownTypesExploded;
        }

        private IEnumerable<Type> ExplodeKnownTypes(IEnumerable<Type> types)
        {
            return KnownTypes.Explode(
                types, this.AutoAddKnownTypesCollectionType);
        }
    }
}
