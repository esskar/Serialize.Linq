using System.Runtime.Serialization;

namespace Serialize.Linq.Examples.AspNetCore.Entities
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
