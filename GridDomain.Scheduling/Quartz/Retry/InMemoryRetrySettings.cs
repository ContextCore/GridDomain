using System;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public class InMemoryRetrySettings : IRetrySettings
    {
        public InMemoryRetrySettings(int maxTries = 3, TimeSpan? baseInterval = null, IExceptionPolicy errorActions = null)
        {
            ErrorActions = errorActions;
            MaxTries = maxTries;
            BackoffBaseInterval = baseInterval ?? TimeSpan.FromMinutes(20);
            ErrorActions = errorActions ?? new NeverRetryExceptionPolicy();
        }

        public int MaxTries { get; }
        public TimeSpan BackoffBaseInterval { get; }
        public IExceptionPolicy ErrorActions { get; }
    }
}