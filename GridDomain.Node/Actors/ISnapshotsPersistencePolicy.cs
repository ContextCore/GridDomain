using System;
using Akka.Persistence;

namespace GridDomain.Node.Actors
{
    public interface ISnapshotsPersistencePolicy
    {
        bool ShouldSave(DateTime? now=null);
        SnapshotSelectionCriteria GetSnapshotsToDelete();
        void MarkEventsProduced(int amount);
        void MarkSnapshotApplied(SnapshotMetadata metadata);
        void MarkSnapshotSaved(DateTime? saveTime=null);
    }
}