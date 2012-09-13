using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public sealed class SerializerSettings : ISerializerSettings
    {
        public SerializerSettings()
        {
            this.UseAssemblyQualifiedName = true;
            this.UseReferences = false;
        }

        public bool UseAssemblyQualifiedName { get; set; }

        public bool UseReferences { get; set; }
    }
}
