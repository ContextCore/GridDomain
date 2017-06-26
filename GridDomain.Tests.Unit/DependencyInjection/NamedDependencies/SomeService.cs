namespace GridDomain.Tests.Unit.DependencyInjection.NamedDependencies
{
    public class SomeService
    {
        public readonly string Name;

        public SomeService(string name)
        {
            Name = name;
        }
    }
}