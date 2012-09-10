using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serialize.Linq.Extensions;

namespace Serialize.Linq.Internals
{
    internal class MemberTypeEnumerator : IEnumerator<Type>
    {
        private int _currentIndex;
        private readonly Type _type;
        private readonly BindingFlags _bindingFlags;
        private readonly HashSet<Type> _seenTypes;
        private Type[] _allTypes;

        public MemberTypeEnumerator(Type type, BindingFlags bindingFlags = BindingFlags.Default)
            : this(new HashSet<Type>(), type, bindingFlags) { }

        public MemberTypeEnumerator(HashSet<Type> seenTypes, Type type, BindingFlags bindingFlags)
        {
            if(seenTypes == null)
                throw new ArgumentNullException("seenTypes");
            if(type == null)
                throw new ArgumentNullException("type");

            _seenTypes = seenTypes;
            _type = type;
            _bindingFlags = bindingFlags;

            _currentIndex = -1;
        }

        public bool IsConsidered 
        {
            get { return this.IsConsideredType(_type); }
        }

        protected virtual bool IsConsideredType(Type type)
        {
            return true;
        }

        protected virtual bool IsConsideredMember(MemberInfo member)
        {
            return true;
        }

        protected bool IsSeenType(Type type)
        {
            return _seenTypes.Contains(type);
        }

        protected void AddSeenType(Type type)
        {
            _seenTypes.Add(type);
        }

        public virtual Type Current
        {
            get { return _allTypes[_currentIndex]; }
        }        

        public void Dispose() { }
        
        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }
        
        protected Type[] GetTypesOfType(Type type)
        {
            var types = new List<Type> { type };
            if (type.HasElementType)
                types.AddRange(this.GetTypesOfType(type.GetElementType()));
            if (type.IsGenericType)
            {
                foreach (var genericType in type.GetGenericArguments())
                    types.AddRange(this.GetTypesOfType(genericType));
                
            }
            return types.ToArray();
        }

        protected virtual void BuildTypes()
        {
            var types = new List<Type>();
            var members = _type.GetMembers(_bindingFlags);
            foreach (var memberInfo in members.Where(this.IsConsideredMember))
                types.AddRange(this.GetTypesOfType(memberInfo.GetReturnType()));
            _allTypes = types.ToArray();
        }

        public virtual bool MoveNext()
        {
            if (!this.IsConsidered)
                return false;

            if (_allTypes == null)
                this.BuildTypes();

            while (++_currentIndex < _allTypes.Length)
            {                
                if (this.IsSeenType(this.Current)) continue;
                this.AddSeenType(this.Current);
                if (this.IsConsideredType(this.Current)) break;
            }

            return _currentIndex < _allTypes.Length;
        }

        public void Reset()
        {
            _currentIndex = -1;
        }        
    }
}