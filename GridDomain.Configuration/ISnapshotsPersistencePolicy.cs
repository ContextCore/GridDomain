using System;

namespace GridDomain.Configuration
{
    public interface ISnapshotsPersistencePolicy
    {
        bool ShouldSave(long snapshotSequenceNr, DateTime? now = null);
        bool ShouldDelete(out SnapshotSelectionCriteria deleteDelegate);

        void MarkSnapshotApplied(long sequenceNr);
        void MarkSnapshotSaved(long snapshotSequenceNumber, DateTime? saveTime = null);
        void MarkSnapshotSaving();
        bool SnapshotsSaveInProgress { get; }
    }
}