using System;
using System.Linq;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{

    public class SnapshotsSavePolicy : ISnapshotsSavePolicy
    {
        private int _messagesProduced;
        private long _lastSnapshotNumber;
        private readonly int _snapshotsToKeep;
        private DateTime _lastActivityTime;
        private readonly TimeSpan _sleepTime;
        private readonly int _saveOnEach;

        public SnapshotsSavePolicy(TimeSpan sleepTime, int saveOnEach, int snapshotsToKeep = 5)
        {
            _saveOnEach = saveOnEach;
            _sleepTime = sleepTime;
            _snapshotsToKeep = snapshotsToKeep;
        }

        public SnapshotSelectionCriteria SnapshotsToDelete()
        {
            var persistenceId = Math.Max(_lastSnapshotNumber - _snapshotsToKeep,0);
            return new SnapshotSelectionCriteria(persistenceId);
        }

        public void SnapshotWasSaved(SnapshotMetadata metadata)
        {
            _lastSnapshotNumber = metadata.SequenceNr;
        }

        public void SnapshotWasApplied(SnapshotMetadata metadata)
        {
            _lastSnapshotNumber = metadata.SequenceNr;
        }

        public bool ShouldSave(params object[] stateChanges)
        {
            if (!stateChanges.Any()) return false;

            if(_messagesProduced == 0 && _lastActivityTime == default(DateTime))
               RefreshActivity();

            _messagesProduced += stateChanges.Length;
            if ((_messagesProduced % _saveOnEach == 0) || _lastActivityTime + _sleepTime < BusinessDateTime.UtcNow)
                return true;

            RefreshActivity();
            return false;
        }

        public void RefreshActivity(DateTime? lastActivityTime = null)
        {
            _lastActivityTime = lastActivityTime ?? BusinessDateTime.UtcNow;
        }
    }
}