namespace Serialize.Linq.Interfaces
{
    public interface ITextSerializer : ISerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string text);
    }
}
