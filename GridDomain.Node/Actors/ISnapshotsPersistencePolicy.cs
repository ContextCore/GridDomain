using System;
using Akka.Persistence;

namespace GridDomain.Node.Actors
{
    public interface ISnapshotsPersistencePolicy
    {
        bool ShouldSave();
        SnapshotSelectionCriteria GetSnapshotsToDelete();
        void MarkEventsProduced(int amount);
        void MarkSnapshotApplied(SnapshotMetadata metadata);
    }
}