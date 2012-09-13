namespace Serialize.Linq.Interfaces
{
    public interface ISerializerSettings
    {
        bool UseAssemblyQualifiedName { get; set; }

        bool UseReferences { get; set; }
    }
}
