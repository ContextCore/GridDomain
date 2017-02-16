using System;
using Akka.Persistence;

namespace GridDomain.Node.Actors
{
    public class NoSnapshotsPersistencePolicy : ISnapshotsPersistencePolicy
    {
        public bool ShouldSave(DateTime? now = null)
        {
            return false;
        }

        public SnapshotSelectionCriteria GetSnapshotsToDelete()
        {
            return new SnapshotSelectionCriteria(0);
        }

        public void MarkEventsProduced(int amount)
        {
        }

        public void MarkSnapshotApplied(SnapshotMetadata metadata)
        {
        }

        public void MarkSnapshotSaved(DateTime? saveTime = null)
        {
        }
    }
}