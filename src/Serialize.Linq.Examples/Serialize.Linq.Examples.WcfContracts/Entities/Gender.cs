using System.Runtime.Serialization;

namespace Serialize.Linq.Examples.WcfContracts.Entities
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
