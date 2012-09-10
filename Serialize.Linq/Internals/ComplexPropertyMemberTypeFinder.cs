using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serialize.Linq.Internals
{
    internal class ComplexPropertyMemberTypeFinder
    {
        private bool AnalyseTypes(IEnumerable<Type> types, ISet<Type> seen, ISet<Type> result)
        {
            return types != null 
                && types.Aggregate(false, (current, type) => this.BuildTypes(type, seen, result) || current);
        }

        private bool AnalyseType(Type baseType, ISet<Type> seen, ISet<Type> result)
        {
            bool retval;
            if (baseType.HasElementType)
            {
                if (!(retval = this.BuildTypes(baseType.GetElementType(), seen, result)))
                    retval = seen.Contains(baseType.GetElementType());
            }
            else
            {
                retval = true;
            }

            if (baseType.IsGenericType)
                retval = this.AnalyseTypes(baseType.GetGenericArguments(), seen, result) || retval;
            retval = this.AnalyseTypes(baseType.GetInterfaces(), seen, result) || retval;
            if (baseType.BaseType != null && baseType.BaseType != typeof(object))
                retval = this.BuildTypes(baseType.BaseType, seen, result) || retval;
            return retval;
        }

        private bool BuildTypes(Type baseType, ISet<Type> seen, ISet<Type> result)
        {            
            if (seen.Contains(baseType))
                return false;            
            seen.Add(baseType);
            if (!this.AnalyseType(baseType, seen, result))
                return false;

            var enumerator = new ComplexPropertyMemberTypeEnumerator(baseType, BindingFlags.Instance | BindingFlags.Public);
            if (!enumerator.IsConsidered)
                return false;
            result.Add(baseType);

            var retval = false;
            while (enumerator.MoveNext())
            {
                var type = enumerator.Current;
                retval = this.BuildTypes(type, seen, result) || retval;
            }

            return retval;
        }

        public IEnumerable<Type> FindTypes(Type baseType)
        {
            var retval = new HashSet<Type>();
            this.BuildTypes(baseType, new HashSet<Type>(), retval);
            return retval;            
        }
    }
}
