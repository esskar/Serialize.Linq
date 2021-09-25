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
using System.Runtime.Serialization;
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
            this.Initialize(type);
        }

        private void Initialize(Type type)
        {
            Type typeDefinition;
            if (type == null)
                return;
            if (type.IsGenericType())
            {
                this.GenericArguments = type.GetGenericArguments().Select(t => new TypeNode(this.Factory, t)).ToArray();
                typeDefinition = type.GetGenericTypeDefinition();
            }
            else
            {
                typeDefinition = type;
            }
            if (!this.Factory.Settings.UseRelaxedTypeNames || type.IsAnonymous())
                this.Name = typeDefinition.AssemblyQualifiedName;
            else
                this.Name = typeDefinition.FullName;
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
                if (String.IsNullOrWhiteSpace(this.Name))
                    return null;
                throw new SerializationException(String.Format("Failed to serialize '{0}' to a type object.", this.Name));
            }

            if (this.GenericArguments != null)
                type = type.MakeGenericType(this.GenericArguments.Select(t => t.ToType(context)).ToArray());

            return type;
        }
    }
}