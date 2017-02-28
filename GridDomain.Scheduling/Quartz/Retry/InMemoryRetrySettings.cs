using System;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public class InMemoryRetrySettings : IRetrySettings
    {
        public InMemoryRetrySettings(int maxRetries = 3, TimeSpan? baseInterval = null, IExceptionPolicy errorActions = null)
        {
            ErrorActions = errorActions;
            MaxRetries = maxRetries;
            BackoffBaseInterval = baseInterval ?? TimeSpan.FromMinutes(20);
            ErrorActions = errorActions ?? new NeverRetryExceptionPolicy();
        }

        public int MaxRetries { get; }
        public TimeSpan BackoffBaseInterval { get; }
        public IExceptionPolicy ErrorActions { get; }
    }
}