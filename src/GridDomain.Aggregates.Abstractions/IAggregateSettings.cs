using System;

namespace GridDomain.Aggregates
{
    public interface IAggregateSettings
    {
        TimeSpan MaxInactivityPeriod { get; }
        int SnapshotsKeepAmount { get; }
        string HostRole { get; }
    }
}