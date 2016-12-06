using System;
using System.Collections.Generic;

namespace GridDomain.Scheduling.Quartz
{
    public interface IRetrySettings
    {
        int MaxRetries { get; }
        TimeSpan BackoffBaseInterval { get; }

        IReadOnlyCollection<Type> ExceptionsToStop { get; }
    }
}