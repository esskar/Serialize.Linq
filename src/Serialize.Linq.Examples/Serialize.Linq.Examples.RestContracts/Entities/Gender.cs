using System.Runtime.Serialization;

namespace Serialize.Linq.Examples.RestContracts.Entities
{
    [DataContract]
    public enum Gender
    {
        [EnumMember]
        Male,
        [EnumMember]
        Female
    }
}
