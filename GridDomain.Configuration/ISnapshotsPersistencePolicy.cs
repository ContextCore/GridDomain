using System;

namespace GridDomain.Configuration
{
    public class SnapshotSelectionCriteria
    {
        public long MaxSequenceNr;
        public DateTime MaxTimeStamp;
        public long MinSequenceNr;
        public DateTime? MinTimestamp;
    }

    public interface ISnapshotsPersistencePolicy
    {
        bool TryDelete(Action<SnapshotSelectionCriteria> deleteDelegate);
        void MarkSnapshotApplied(long sequenceNr);
        void MarkSnapshotSaved(long snapshotSequenceNumber, DateTime? saveTime = null);
        bool TrySave(Action saveDelegate, long snapshotSequenceNr, DateTime? now = null);
    }
}