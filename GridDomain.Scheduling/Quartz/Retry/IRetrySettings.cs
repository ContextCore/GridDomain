using System;

namespace GridDomain.Scheduling.Quartz
{
    public interface IRetrySettings
    {
        int MaxRetries { get; }
        TimeSpan BackoffBaseInterval { get; }
    }
}