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
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "T")]
#endif
#if !(SILVERLIGHT || DNXCORE50 || DOTNET5_4)
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

#if DNXCORE50 || DOTNET5_4
            bool isAttributeDefined = type.GetTypeInfo().GetCustomAttribute(typeof(CompilerGeneratedAttribute)) != null;
#else
            bool isAttributeDefined = Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false);
#endif
            var isAnonymousType = isAttributeDefined
                && type.GetTypeInfo().IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.GetTypeInfo().Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;

            if (type.GetTypeInfo().IsGenericType)
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

        public Type ToType(ExpressionContext context)
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