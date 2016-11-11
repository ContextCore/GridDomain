using System;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    class SnapshotsSaveOnTimeoutPolicy : SnapshotsSavePolicy
    {
        public SnapshotsSaveOnTimeoutPolicy() : base(TimeSpan.FromSeconds(1), 100)
        {

        }
    }
}