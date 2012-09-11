namespace Serialize.Linq.Interfaces
{
    public interface IFormatSerializer<TFormatType>
    {
        TFormatType Serialize<T>(T obj);
        T Deserialize<T>(TFormatType data);
    }
}
