using System;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    class SnapshotsSaveAfterEachMessagePolicy : SnapshotsSavePolicy
    {
        public SnapshotsSaveAfterEachMessagePolicy(int snapshotsToKeep = 5) : base(TimeSpan.FromSeconds(1000), 1, snapshotsToKeep)
        {

        }
    }
}