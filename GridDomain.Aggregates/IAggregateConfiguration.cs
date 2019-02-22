using System;

namespace GridDomain.Aggregates
{
    public interface IAggregateConfiguration
    {
        TimeSpan MaxInactivityPeriod { get; }
        int SnapshotsKeepAmount { get; }
    }
}