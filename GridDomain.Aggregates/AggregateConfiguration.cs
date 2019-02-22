using System;

namespace GridDomain.Aggregates
{
    public class AggregateConfiguration : IAggregateConfiguration
    {
        public TimeSpan MaxInactivityPeriod { get; } = TimeSpan.FromMinutes(30);
        public int SnapshotsKeepAmount { get; } = 5;
    }
}