#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Serialize.Linq.Extensions;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "T")]
#endif
#if !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class TypeNode : Node
    {        
        public TypeNode() { }

        public TypeNode(INodeFactory factory, Type type)
            : base(factory)
        {
            Initialize(type);
        }

        private void Initialize(Type type)
        {
            if (type == null)
                return;

            var isAnonymousType = type.IsCustomAttributeDefined(typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType() && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.GetTypeAttributes() & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;

            if (type.IsGenericType())
            {
                GenericArguments = type.GetGenericArguments().Select(t => new TypeNode(Factory, t)).ToArray();

                var typeDefinition = type.GetGenericTypeDefinition();
                if (isAnonymousType || !Factory.Settings.UseRelaxedTypeNames)
                    Name = typeDefinition.AssemblyQualifiedName;
                else
                    Name = typeDefinition.FullName;
            }
            else
            {
                if (isAnonymousType || !Factory.Settings.UseRelaxedTypeNames)
                    Name = type.AssemblyQualifiedName;
                else
                    Name = type.FullName;
            }            
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "N")]        
#endif
        #endregion
        public string Name { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "G")]        
#endif
        #endregion
        public TypeNode[] GenericArguments { get; set; }

        public Type ToType(IExpressionContext context)
        {
            var type = context.ResolveType(this);
            if (type == null)
            {
                if (string.IsNullOrWhiteSpace(Name))
                    return null;
                throw new SerializationException(string.Format("Failed to serialize '{0}' to a type object.", Name));
            }

            if (GenericArguments != null)            
                type = type.MakeGenericType(GenericArguments.Select(t => t.ToType(context)).ToArray());
            
            return type;
        }
    }
}