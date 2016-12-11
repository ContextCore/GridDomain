using System;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    public class TestScheduledException : Exception
    {
        public int RetriesToSucceed { get; }

        public TestScheduledException(int retriesToSucceed):base($"Intentional test exception was reised, retry {retriesToSucceed} times to succeed")
        {
            RetriesToSucceed = retriesToSucceed;
        }
    }
}