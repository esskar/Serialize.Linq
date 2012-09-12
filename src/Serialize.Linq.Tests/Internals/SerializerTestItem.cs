using System.Linq.Expressions;

namespace Serialize.Linq.Tests.Internals
{
    public class SerializerTestItem
    {
        public Expression Expression { get; set; }

        public string Json { get; set; }

        public string Xml { get; set; }
    }
}
