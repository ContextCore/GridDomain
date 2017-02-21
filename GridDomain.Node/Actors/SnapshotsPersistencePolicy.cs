using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Event;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.EventSourcing;
using Serilog;

namespace GridDomain.Node.Actors
{
    public class SnapshotsPersistencePolicy : ISnapshotsPersistencePolicy
    {
        private long _lastSequenceNumber;
        private DateTime _savedAt;
        private long _lastSavedSnapshot;
        private readonly int _eventsToKeep;
        private readonly int _saveOnEach;
        private readonly TimeSpan? _maxSaveFrequency;

        public ILogger Log { get; set; }

        public SnapshotsPersistencePolicy(int saveOnEach = 1,
                                          int eventsToKeep = 10,
                                          TimeSpan? maxSaveFrequency = null,
                                          DateTime? savedAt = null)
        {
            _maxSaveFrequency = maxSaveFrequency ?? TimeSpan.MinValue;
            _saveOnEach = saveOnEach;
            _eventsToKeep = eventsToKeep;
            _savedAt = savedAt ?? DateTime.MinValue;
        }

        public bool TrySave(Action saveDelegate, long snapshotSequenceNr, DateTime? now = null)
        {
            bool saveIsInTime = (now ?? BusinessDateTime.UtcNow) - _savedAt >= _maxSaveFrequency;
            _lastSequenceNumber = Math.Max(snapshotSequenceNr, _lastSequenceNumber);

            if (!saveIsInTime)
            {
                Log?.Debug("will not save snapshots due to time limitations");
                return false;
            }
            if (_lastSequenceNumber %_saveOnEach != 0)
            {
                Log?.Debug("will not save snapshots due to save on event count condition");
                return false;
            }

            Log?.Debug("Saving snapshot {num}", snapshotSequenceNr);
            saveDelegate();

            return true;
        }

        public bool TryDelete(Action<SnapshotSelectionCriteria> deleteDelegate)
        {
            var maxSnapshotNumToDelete = Math.Max(_lastSavedSnapshot - _eventsToKeep, 0);
            var criteria = new SnapshotSelectionCriteria(maxSnapshotNumToDelete);
            deleteDelegate(criteria);
            return true;
        }

        public void MarkSnapshotApplied(long sequenceNr)
        {
            _lastSequenceNumber = sequenceNr;
            _lastSavedSnapshot = sequenceNr;
        }

        public void MarkSnapshotSaved(long snapshotsSequenceNumber, DateTime? saveTime = null)
        {
            Log?.Debug("Snapshot {num} saved", snapshotsSequenceNumber);
            _savedAt = saveTime ?? BusinessDateTime.UtcNow;
            _lastSavedSnapshot = snapshotsSequenceNumber;
        }
    }
}