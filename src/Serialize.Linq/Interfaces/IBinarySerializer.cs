namespace Serialize.Linq.Interfaces
{
    public interface IBinarySerializer : ISerializer
    {
        byte[] Serialize<T>(T obj);
        T Deserialize<T>(byte[] bytes);
    }
}
