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
        private bool _autoDiscoverKnownTypes;
        private IEnumerable<Type> _knownTypesExploded;

        protected SerializerBase()
        {
            _customKnownTypes = new HashSet<Type>();
            AutoAddKnownTypesAsArrayTypes = true;
        }

        public bool AutoAddKnownTypesAsArrayTypes
        {
            get => _autoAddKnownTypesAsArrayTypes;
            set
            {
                _autoAddKnownTypesAsArrayTypes = value;
                if (value)
                    _autoAddKnownTypesAsListTypes = false;
                _knownTypesExploded = null;
            }
        }

        public bool AutoAddKnownTypesAsListTypes
        {
            get => _autoAddKnownTypesAsListTypes;
            set
            {
                _autoAddKnownTypesAsListTypes = value;
                if (value)
                    _autoAddKnownTypesAsArrayTypes = false;
                _knownTypesExploded = null;
            }
        }

        /// <summary>
        /// If set to true, the runtime types of constant values encountered while serializing an
        /// expression are automatically registered as known types. This removes the need to call
        /// <see cref="AddKnownType"/> manually for custom types or enum values used inside an expression.
        /// </summary>
        public bool AutoDiscoverKnownTypes
        {
            get => _autoDiscoverKnownTypes;
            set => _autoDiscoverKnownTypes = value;
        }

        public void AddKnownType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (_customKnownTypes.Add(type))
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
            if (_knownTypesExploded != null)
                return _knownTypesExploded;

            _knownTypesExploded = ExplodeKnownTypes(KnownTypes.All)
                .Concat(ExplodeKnownTypes(_customKnownTypes)).ToList();
            return _knownTypesExploded;
        }

        private IEnumerable<Type> ExplodeKnownTypes(IEnumerable<Type> types)
        {
            return KnownTypes.Explode(
                types, AutoAddKnownTypesAsArrayTypes, AutoAddKnownTypesAsListTypes);
        }
    }
}
