using System;

namespace GridDomain.Tests.DependencyInjection
{
    public class TestDependencyImplementation : ITestDependency
    {
        public string Do(int param)
        {
            throw new NotImplementedException();
        }
    }
}