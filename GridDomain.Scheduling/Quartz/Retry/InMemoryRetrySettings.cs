using System;

namespace GridDomain.Scheduling.Quartz
{
    public class InMemoryRetrySettings : IRetrySettings
    {
        public int MaxRetries { get; }
        public TimeSpan BackoffBaseInterval { get; }

        public InMemoryRetrySettings(int maxRetries = 5, TimeSpan? baseInterval = null)
        {
            MaxRetries = maxRetries;
            BackoffBaseInterval = baseInterval ?? TimeSpan.FromMinutes(1);
        }
    }
}