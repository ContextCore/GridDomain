using System;

namespace GridDomain.Configuration
{
    public interface ISnapshotsPersistencePolicy
    {
        bool ShouldSave(long snapshotSequenceNr, DateTime? now = null);
        bool ShouldDelete(out SnapshotSelectionCriteria deleteDelegate);

        //called on aggregte restore to set up initinal sequence numbers
        void MarkSnapshotApplied(long sequenceNr);
        void MarkSnapshotSaved(long snapshotSequenceNumber, DateTime? saveTime = null);

        //called on aggregte snapshot save start
        void MarkSnapshotSaving(DateTime? now = null);
        bool SnapshotsSaveInProgress { get; }
    }
}