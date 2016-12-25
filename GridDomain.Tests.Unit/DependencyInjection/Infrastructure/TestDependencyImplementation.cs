namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure
{
    public class TestDependencyImplementation : ITestDependency
    {
        public string Do(int param)
        {
            return param.ToString();
        }
    }
}