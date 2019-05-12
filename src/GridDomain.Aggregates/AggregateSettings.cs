using System;

namespace GridDomain.Aggregates
{
    public class AggregateSettings : IAggregateSettings
    {
        public AggregateSettings(TimeSpan? maxInactivityPeriod=null, int? snapshotsKeepAmount=null, string hostRole=null)
        {
            MaxInactivityPeriod = maxInactivityPeriod ?? TimeSpan.FromMinutes(30);
            SnapshotsKeepAmount = snapshotsKeepAmount ?? 5;
            HostRole = hostRole;
        }
        public TimeSpan MaxInactivityPeriod { get; }
        public int SnapshotsKeepAmount { get; }
        public string HostRole { get; }
    }
}