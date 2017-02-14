using System;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    class SnapshotsPersistenceAfterEachMessagePolicy : SnapshotsPersistencePolicy
    {
        public SnapshotsPersistenceAfterEachMessagePolicy(int eventsToStore = 5) : base(TimeSpan.FromSeconds(1000), 1, eventsToStore)
        {

        }
    }
}