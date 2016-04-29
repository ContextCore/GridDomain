using System;
using System.Data.Entity.Infrastructure;
using System.Threading;

namespace GridDomain.CQRS.ReadModel
{
    //TODO: create a waitable version with increasing delay
    public class RetryPolicy<TException> where TException : Exception
    {
        private readonly int _maxRetryCount;

        public RetryPolicy(int maxRetryCount)
        {
            _maxRetryCount = maxRetryCount;
        }

        public void Execute(Action act)
        {
            int tryCount = 0;

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
                        throw new RetryLimitExceededException($"Reached retry limit {_maxRetryCount}", ex);
                }
            }
            while (tryCount < _maxRetryCount);
        }

        public static RetryPolicy<TException> DefaultSql()
        {
            return new RetryPolicy<TException>(5);
        }
    }
}