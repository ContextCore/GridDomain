using System;

namespace GridDomain.Aggregates
{
    public class AggregateConfiguration : IAggregateConfiguration
    {
        public AggregateConfiguration(TimeSpan? maxInactivityPeriod=null, int? snapshotsKeepAmount=null)
        {
            MaxInactivityPeriod = maxInactivityPeriod ?? TimeSpan.FromMinutes(30);
            SnapshotsKeepAmount = snapshotsKeepAmount ?? 5;
        }
        public TimeSpan MaxInactivityPeriod { get; }
        public int SnapshotsKeepAmount { get; }
    }
}