using System;

namespace GridDomain.Configuration
{
    public class NoSnapshotsPersistencePolicy : ISnapshotsPersistencePolicy
    {
        public bool ShouldDelete(out SnapshotSelectionCriteria deleteDelegate)
        {
            deleteDelegate = new SnapshotSelectionCriteria();
            return false;
        }

        public void MarkSnapshotApplied(long sequenceNr) {}

        public void MarkSnapshotSaved(long snapshotSequenceNumber, DateTime? saveTime = null) {}

        public bool ShouldSave(long snapshotSequenceNr, DateTime? now = null)
        {
            return false;
        }
    }
}