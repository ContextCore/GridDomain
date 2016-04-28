using System;
using System.Data.Entity.Infrastructure;
using System.Threading;

namespace GridDomain.CQRS.ReadModel
{
    public class WaitableRetryPolicy<TException> where TException : Exception
    {
        private readonly int _maxRetryCount;
        private readonly double _retryTimeMultiply;
        private readonly TimeSpan _defaultTimeToWait;

        public WaitableRetryPolicy(int maxRetryCount, double retryTimeMultiply, TimeSpan defaultTimeToWait)
        {
            _defaultTimeToWait = defaultTimeToWait;
            _retryTimeMultiply = retryTimeMultiply;
            _maxRetryCount = maxRetryCount;
        }

        public void Execute(Action act)
        {
            int tryCount = 0;
            TimeSpan timeToWait = _defaultTimeToWait;

            do
            {
                try
                {
                    tryCount++;
                    act();
                    break;
                }
                catch (TException ex)
                {
                    if (tryCount >= _maxRetryCount)
                        throw new RetryLimitExceededException();

                    //TODO: remove and change to message notification
                    Thread.Sleep(timeToWait);
                    timeToWait = TimeSpan.FromMilliseconds(timeToWait.TotalMilliseconds * _retryTimeMultiply);
                }
            } while (tryCount < _maxRetryCount);
        }

        public static WaitableRetryPolicy<TException> DefaultSql()
        {
            return new WaitableRetryPolicy<TException>(5, 2, TimeSpan.FromMilliseconds(100));
        }
    }
}