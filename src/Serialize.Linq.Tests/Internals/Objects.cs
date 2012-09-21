namespace Serialize.Linq.Tests.Internals
{
    public interface IFoo
    {
        string Name { get; set; }
    }

    public class Foo : IFoo
    {
        public string Name { get; set; }
    }

    public class Bar
    {
        public bool IsFoo;

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string GetName()
        {
            return string.Format("{0} {1}", this.FirstName, this.LastName);
        }
    }

    public class Fish
    {
        public int? Count { get; set; }
    }
}
