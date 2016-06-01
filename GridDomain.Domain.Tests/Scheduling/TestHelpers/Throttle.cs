using System;
using System.Linq.Expressions;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public static class Throttle
    {
        public static void Verify<T>(Mock<T> mock, Expression<Action<T>> verification, TimeSpan minTimeout = default(TimeSpan), TimeSpan maxTimeout = default(TimeSpan)) where T : class
        {
            maxTimeout = maxTimeout == default(TimeSpan) ? TimeSpan.FromSeconds(5) : maxTimeout;
            var cancellationToken = new CancellationTokenSource(maxTimeout).Token;
            if (minTimeout != default(TimeSpan))
            {
                Thread.Sleep(minTimeout);
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    mock.Verify(verification);
                    NUnit.Framework.Assert.True(true, "method was called");
                    return;
                }
                catch (MockException)
                {
                }
            }
            throw new AssertionException("method wasn`t called");
        }

        public static void Assert(Action action, TimeSpan minTimeout = default(TimeSpan), TimeSpan maxTimeout = default(TimeSpan))
        {
            maxTimeout = maxTimeout == default(TimeSpan) ? TimeSpan.FromSeconds(5) : maxTimeout;
            var cancellationToken = new CancellationTokenSource(maxTimeout).Token;
            if (minTimeout != default(TimeSpan))
            {
                Thread.Sleep(minTimeout);
            }
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