using System;
using Akka.Persistence;
using GridDomain.Configuration;
using SnapshotSelectionCriteria = GridDomain.Configuration.SnapshotSelectionCriteria;

namespace GridDomain.Node.Actors.EventSourced
{
    public class NoSnapshotsPersistencePolicy : ISnapshotsPersistencePolicy
    {
        public bool TryDelete(Action<SnapshotSelectionCriteria> deleteDelegate)
        {
            return false;
        }

        public void MarkSnapshotApplied(long sequenceNr) {}

        public void MarkSnapshotSaved(long snapshotSequenceNumber, DateTime? saveTime = null) {}

        public bool TrySave(Action saveDelegate, long snapshotSequenceNr, DateTime? now = null)
        {
            return true;
        }
    }
}