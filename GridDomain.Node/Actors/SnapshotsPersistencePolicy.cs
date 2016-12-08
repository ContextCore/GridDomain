using System;
using System.Linq;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{

    public class SnapshotsPersistencePolicy : ISnapshotsPersistencePolicy
    {
        private int _messagesProduced;
        private long _lastSequenceNumber;
        private readonly int _eventsToKeep;
        private DateTime _lastActivityTime;
        private readonly TimeSpan _sleepTime;
        private readonly int _saveOnEach;

        public SnapshotsPersistencePolicy(TimeSpan sleepTime, int saveOnEach=2, int eventsToKeep = 10)
        {
            _saveOnEach = saveOnEach;
            _sleepTime = sleepTime;
            _eventsToKeep = eventsToKeep;
            MarkActivity();
        }

        public SnapshotSelectionCriteria GetSnapshotsToDelete()
        {
            var persistenceId = Math.Max(_lastSequenceNumber - _eventsToKeep + 1, 0);
            return new SnapshotSelectionCriteria(persistenceId);
        }

        public void MarkSnapshotSaved(SnapshotMetadata metadata)
        {
            _lastSequenceNumber = metadata.SequenceNr;
        }

        public void MarkSnapshotApplied(SnapshotMetadata metadata)
        {
            _lastSequenceNumber = metadata.SequenceNr;
        }

        public bool ShouldSave(params object[] stateChanges)
        {
            if (!stateChanges.Any()) return false;

            _messagesProduced += stateChanges.Length;

            var now = BusinessDateTime.UtcNow;

            if ((_messagesProduced % _saveOnEach == 0) || now -_lastActivityTime > _sleepTime)
                return true;

            MarkActivity();
            return false;
        }

        public void MarkActivity(DateTime? lastActivityTime = null)
        {
            _lastActivityTime = lastActivityTime ?? BusinessDateTime.UtcNow;
        }
    }
}