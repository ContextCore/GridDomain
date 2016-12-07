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

        public SnapshotsPersistencePolicy(TimeSpan sleepTime, int saveOnEach, int eventsToKeep = 10)
        {
            _saveOnEach = saveOnEach;
            _sleepTime = sleepTime;
            _eventsToKeep = eventsToKeep;
        }

        public SnapshotSelectionCriteria GetSnapshotsToDelete()
        {
            var persistenceId = Math.Max(_lastSequenceNumber - _eventsToKeep,0);
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

            if(_messagesProduced == 0 && _lastActivityTime == default(DateTime))
               MarkActivity();

            _messagesProduced += stateChanges.Length;
            if ((_messagesProduced % _saveOnEach == 0) || _lastActivityTime + _sleepTime < BusinessDateTime.UtcNow)
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