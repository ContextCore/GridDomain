using System;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public interface IRetrySettings
    {
        int MaxRetries { get; }
        TimeSpan BackoffBaseInterval { get; }
        IExceptionPolicy ErrorActions { get; }
    }
}