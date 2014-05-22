using System;
using System.Runtime.Serialization;

namespace Serialize.Linq.Examples.WcfContracts.Entities
{
    [DataContract]
    public class Person
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public int Age { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        [DataMember]
        public DateTime BirthDate { get; set; }

        [DataMember]
        public DateTime? DeathDate { get; set; }

        [DataMember]
        public string Residence { get; set; }
    }
}
