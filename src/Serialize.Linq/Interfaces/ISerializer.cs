using System.IO;

namespace Serialize.Linq.Interfaces
{
    public interface ISerializer
    {
        void Serialize<T>(Stream stream, T obj);
        T Deserialize<T>(Stream stream);
    }
}
