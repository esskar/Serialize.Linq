#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "EI")]
#endif
#if !SILVERLIGHT && !NETCOREAPP1_1 && !NETFX_CORE && !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class ElementInitNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementInitNode"/> class.
        /// </summary>
        public ElementInitNode() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementInitNode"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="elementInit">The element init.</param>
        public ElementInitNode(INodeFactory factory, ElementInit elementInit)
            : base(factory)
        {
            this.Initialize(elementInit);
        }

        /// <summary>
        /// Initializes the specified element init.
        /// </summary>
        /// <param name="elementInit">The element init.</param>
        /// <exception cref="System.ArgumentNullException">elementInit</exception>
        private void Initialize(ElementInit elementInit)
        {
            if (elementInit == null)
                throw new ArgumentNullException("elementInit");

            this.AddMethod = new MethodInfoNode(this.Factory, elementInit.AddMethod);
            this.Arguments = new ExpressionNodeList(this.Factory, elementInit.Arguments);
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "A")]
#endif
        #endregion
        public ExpressionNodeList Arguments { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets the add method.
        /// </summary>
        /// <value>
        /// The add method.
        /// </value>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "M")]
#endif
        #endregion
        public MethodInfoNode AddMethod { get; set; }

        internal ElementInit ToElementInit(IExpressionContext context)
        {
            return Expression.ElementInit(this.AddMethod.ToMemberInfo(context), this.Arguments.GetExpressions(context));
        }
    }
}
