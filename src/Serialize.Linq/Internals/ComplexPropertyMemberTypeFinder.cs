
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serialize.Linq.Internals
{
    internal class ComplexPropertyMemberTypeFinder
    {
        /// <summary>
        /// Analyses the types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="seen">The seen.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private bool AnalyseTypes(IEnumerable<Type> types, ISet<Type> seen, ISet<Type> result)
        {
            return types != null 
                && types.Aggregate(false, (current, type) => BuildTypes(type, seen, result) || current);
        }

        /// <summary>
        /// Analyses the type.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="seen">The seen.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private bool AnalyseType(Type baseType, ISet<Type> seen, ISet<Type> result)
        {
            bool retval;
            if (baseType.HasElementType)
            {
                if (!(retval = BuildTypes(baseType.GetElementType(), seen, result)))
                    retval = seen.Contains(baseType.GetElementType());
            }
            else
            {
                retval = true;
            }

            if (baseType.IsGenericType())
                retval = AnalyseTypes(baseType.GetGenericArguments(), seen, result) || retval;
            retval = AnalyseTypes(baseType.GetInterfaces(), seen, result) || retval;
            var subBaseType = baseType.GetBaseType();
            if (subBaseType != null && subBaseType != typeof(object))
                retval = BuildTypes(subBaseType, seen, result) || retval;
            return retval;
        }

        /// <summary>
        /// Builds the types.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <param name="seen">The seen.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private bool BuildTypes(Type baseType, ISet<Type> seen, ISet<Type> result)
        {            
            if (seen.Contains(baseType))
                return false;            
            seen.Add(baseType);
            if (!AnalyseType(baseType, seen, result))
                return false;

            var enumerator = new ComplexPropertyMemberTypeEnumerator(baseType, BindingFlags.Instance | BindingFlags.Public);
            if (!enumerator.IsConsidered)
                return false;
            result.Add(baseType);

            var retval = false;
            while (enumerator.MoveNext())
            {
                var type = enumerator.Current;
                retval = BuildTypes(type, seen, result) || retval;
            }

            return retval;
        }

        /// <summary>
        /// Finds the types.
        /// </summary>
        /// <param name="baseType">Type of the base.</param>
        /// <returns></returns>
        public IEnumerable<Type> FindTypes(Type baseType)
        {
            var retval = new HashSet<Type>();
            BuildTypes(baseType, new HashSet<Type>(), retval);
            return retval;            
        }
    }
}
