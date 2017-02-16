using System;
using System.Linq;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{

    public class SnapshotsPersistencePolicy : ISnapshotsPersistencePolicy
    {
        private long _lastSequenceNumber;
        private readonly int _eventsToKeep;
        private readonly int _saveOnEach;

        public SnapshotsPersistencePolicy(int saveOnEach=1, int eventsToKeep = 10)
        {
            _saveOnEach = saveOnEach;
            _eventsToKeep = eventsToKeep;
        }

        public SnapshotSelectionCriteria GetSnapshotsToDelete()
        {
            var persistenceId = Math.Max(_lastSequenceNumber - _eventsToKeep, 0);
            return new SnapshotSelectionCriteria(persistenceId);
        }

        public void MarkEventsProduced(int amount)
        {
            _lastSequenceNumber += amount;
        }

        public void MarkSnapshotApplied(SnapshotMetadata metadata)
        {
            _lastSequenceNumber = metadata.SequenceNr;
        }

        public bool ShouldSave()
        {
            return _lastSequenceNumber % _saveOnEach == 0;
        }
    }
}