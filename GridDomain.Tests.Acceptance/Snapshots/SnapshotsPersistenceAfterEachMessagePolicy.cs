using GridDomain.Node.Actors;
using GridDomain.Node.Actors.EventSourced;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    internal class SnapshotsPersistenceAfterEachMessagePolicy : SnapshotsPersistencePolicy
    {
        public SnapshotsPersistenceAfterEachMessagePolicy(int eventsToStore = 5) : base(1, eventsToStore) {}
    }
}