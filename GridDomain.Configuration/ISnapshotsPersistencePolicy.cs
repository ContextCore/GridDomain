using System;
using Akka.Persistence;

namespace GridDomain.Configuration
{
    public interface ISnapshotsPersistencePolicy
    {
        bool TryDelete(Action<SnapshotSelectionCriteria> deleteDelegate);
        void MarkSnapshotApplied(long sequenceNr);
        void MarkSnapshotSaved(long snapshotSequenceNumber, DateTime? saveTime = null);
        bool TrySave(Action saveDelegate, long snapshotSequenceNr, DateTime? now = null);
    }
}