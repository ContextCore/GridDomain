using System;

namespace GridDomain.Aggregates.Abstractions
{
    public interface IAggregateSettings
    {
        TimeSpan MaxInactivityPeriod { get; }
        int SnapshotsKeepAmount { get; }
        string HostRole { get; }
    }
}