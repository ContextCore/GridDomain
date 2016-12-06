using System;
using System.Collections.Generic;

namespace GridDomain.Scheduling.Quartz
{
    public class InMemoryRetrySettings : IRetrySettings
    {
        public int MaxRetries { get; }
        public TimeSpan BackoffBaseInterval { get; }
        public IReadOnlyCollection<Type> ExceptionsToStop { get; }

        public InMemoryRetrySettings(int maxRetries = 5, TimeSpan? baseInterval = null, params Type [] exceptionsToStopRetry)
        {
            MaxRetries = maxRetries;
            BackoffBaseInterval = baseInterval ?? TimeSpan.FromMinutes(20);
            ExceptionsToStop = exceptionsToStopRetry ?? new Type[] {};
        }
    }
}