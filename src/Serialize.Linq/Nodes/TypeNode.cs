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

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "T")]
#endif
#if !SILVERLIGHT && !NETCOREAPP1_1 && !NETFX_CORE && !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class TypeNode : Node
    {        
        public TypeNode() { }

        public TypeNode(INodeFactory factory, Type type)
            : base(factory)
        {
            this.Initialize(type);
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
                this.GenericArguments = type.GetGenericArguments().Select(t => new TypeNode(this.Factory, t)).ToArray();

                var typeDefinition = type.GetGenericTypeDefinition();
                if (isAnonymousType || !this.Factory.Settings.UseRelaxedTypeNames)
                    this.Name = typeDefinition.AssemblyQualifiedName;
                else
                    this.Name = typeDefinition.FullName;
            }
            else
            {
                if (isAnonymousType || !this.Factory.Settings.UseRelaxedTypeNames)
                    this.Name = type.AssemblyQualifiedName;
                else
                    this.Name = type.FullName;
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
                if (string.IsNullOrWhiteSpace(this.Name))
                    return null;
                throw new SerializationException(string.Format("Failed to serialize '{0}' to a type object.", this.Name));
            }

            if (this.GenericArguments != null)            
                type = type.MakeGenericType(this.GenericArguments.Select(t => t.ToType(context)).ToArray());
            
            return type;
        }
    }
}