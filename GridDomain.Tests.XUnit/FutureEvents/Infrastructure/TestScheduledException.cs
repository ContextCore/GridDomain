using System;

namespace GridDomain.Tests.XUnit.FutureEvents.Infrastructure
{
    public class TestScheduledException : Exception
    {
        public TestScheduledException(int retriesToSucceed)
            : base($"Intentional test exception was raised, retry {retriesToSucceed} times to succeed")
        {
            RetriesToSucceed = retriesToSucceed;
        }

        public int RetriesToSucceed { get; }
    }
}