using System;

namespace GridDomain.Scheduling.Quartz.Retry
{
    public interface IRetrySettings
    {
        int MaxTries { get; }
        TimeSpan BackoffBaseInterval { get; }
        IExceptionPolicy ErrorActions { get; }
    }
}