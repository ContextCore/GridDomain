using System;

namespace GridDomain.Configuration
{
    public interface ISnapshotsPersistencePolicy
    {
        bool ShouldDelete(out SnapshotSelectionCriteria deleteDelegate);
        void MarkSnapshotApplied(long sequenceNr);
        void MarkSnapshotSaved(long snapshotSequenceNumber, DateTime? saveTime = null);
        bool ShouldSave(long snapshotSequenceNr, DateTime? now = null);
    }
}