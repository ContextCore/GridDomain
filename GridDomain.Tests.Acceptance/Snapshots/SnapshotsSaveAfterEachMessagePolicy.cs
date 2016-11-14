using System;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    class SnapshotsSaveAfterEachMessagePolicy : SnapshotsSavePolicy
    {
        public SnapshotsSaveAfterEachMessagePolicy() : base(TimeSpan.FromSeconds(1000), 1)
        {

        }
    }
}