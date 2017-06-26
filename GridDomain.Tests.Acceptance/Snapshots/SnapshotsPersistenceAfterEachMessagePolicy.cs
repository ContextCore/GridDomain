using GridDomain.Node.Actors;

namespace GridDomain.Tests.Acceptance.XUnit.Snapshots
{
    internal class SnapshotsPersistenceAfterEachMessagePolicy : SnapshotsPersistencePolicy
    {
        public SnapshotsPersistenceAfterEachMessagePolicy(int eventsToStore = 5) : base(1, eventsToStore) {}
    }
}