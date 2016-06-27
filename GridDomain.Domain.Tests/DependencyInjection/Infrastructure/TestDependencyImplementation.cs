namespace GridDomain.Tests.DependencyInjection.Infrastructure
{
    public class TestDependencyImplementation : ITestDependency
    {
        public string Do(int param)
        {
            return param.ToString();
        }
    }
}