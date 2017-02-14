using System;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    class SnapshotsPersistenceOnTimeoutPolicy : SnapshotsPersistencePolicy
    {
        public SnapshotsPersistenceOnTimeoutPolicy() : base(TimeSpan.FromSeconds(1), 100)
        {

        }
    }
}