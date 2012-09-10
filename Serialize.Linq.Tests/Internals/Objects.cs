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
        public string Name { get; set; }

        public string GetName()
        {
            return this.Name;
        }
    }

    public class Fish
    {
        public int? Count { get; set; }
    }
}
