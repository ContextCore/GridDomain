using System;
using System.Linq.Expressions;
using System.Threading;
using Moq;
using Xunit;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public static class Throttle
    {
        public static void Verify<T>(Mock<T> mock,
                                     Expression<Action<T>> verification,
                                     TimeSpan minTimeout = default(TimeSpan),
                                     TimeSpan maxTimeout = default(TimeSpan)) where T : class
        {
            maxTimeout = maxTimeout == default(TimeSpan) ? TimeSpan.FromSeconds(5) : maxTimeout;
            var cancellationToken = new CancellationTokenSource(maxTimeout).Token;
            if (minTimeout != default(TimeSpan)) { Thread.Sleep(minTimeout); }
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    mock.Verify(verification);
                    Assert.True(true, "method was called");
                    return;
                }
                catch (MockException) {}
                Thread.Sleep(10);
            }
            throw new Exception("method wasn`t called");
        }

        public static void AssertInTime(Action action,
                                        TimeSpan minTimeout = default(TimeSpan),
                                        TimeSpan maxTimeout = default(TimeSpan))
        {
            maxTimeout = maxTimeout == default(TimeSpan) ? TimeSpan.FromSeconds(5) : maxTimeout;
            var cancellationToken = new CancellationTokenSource(maxTimeout).Token;
            if (minTimeout != default(TimeSpan)) { Thread.Sleep(minTimeout); }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception) {}
                Thread.Sleep(10);
            }
            throw new Exception("Assertion wasn`t satisfied in time");
        }
    }
}