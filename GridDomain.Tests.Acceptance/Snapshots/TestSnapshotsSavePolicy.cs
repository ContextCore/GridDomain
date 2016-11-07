using System;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    class TestSnapshotsSavePolicy : SnapshotsSavePolicy
    {
        public TestSnapshotsSavePolicy() : base(TimeSpan.FromSeconds(1), 100)
        {

        }
    }
}