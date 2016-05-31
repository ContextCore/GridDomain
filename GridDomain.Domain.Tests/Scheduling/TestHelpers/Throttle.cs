using System;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public static class Throttle
    {
        public static void Verify(Action verifyMock, TimeSpan timeout = default(TimeSpan))
        {
            timeout = timeout == default(TimeSpan) ? TimeSpan.FromSeconds(3) : timeout;
            var cancellationToken = new CancellationTokenSource(timeout).Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    verifyMock();
                    NUnit.Framework.Assert.True(true, "method was called");
                    return;
                }
                catch (MockException)
                {
                }
            }
            throw new AssertionException("method wasn`t called");
        }

        public static void Assert(Action action, TimeSpan timeout = default(TimeSpan))
        {
            timeout = timeout == default(TimeSpan) ? TimeSpan.FromSeconds(3) : timeout;
            var cancellationToken = new CancellationTokenSource(timeout).Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    action();
                    return;
                }
                catch (AssertionException)
                {
                }
            }
            throw new AssertionException("Assertion wasn`t satisfied in time");
        }
    }
}