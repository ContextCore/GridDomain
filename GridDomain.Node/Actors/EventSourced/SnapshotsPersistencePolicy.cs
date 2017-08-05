using System;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.Configuration;
using Serilog;
using SnapshotSelectionCriteria = GridDomain.Configuration.SnapshotSelectionCriteria;

namespace GridDomain.Node.Actors.EventSourced
{
    public class SnapshotsPersistencePolicy : ISnapshotsPersistencePolicy
    {
        private readonly int _eventsToKeep;
        private readonly TimeSpan? _maxSaveFrequency;
        private readonly int _saveOnEach;
        private long _lastSavedSnapshot;
        private long _lastSnapshotAppliedNumber;
        private DateTime _savedAt;

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

        public void MarkSnapshotSaving()
        {
            _snapshotsSaveInProgressCount++;
        }

        public bool ShouldSave(long snapshotSequenceNr, DateTime? now = null)
        {
            var saveIsInTime = (now ?? BusinessDateTime.UtcNow) - _savedAt >= _maxSaveFrequency;
            snapshotSequenceNr = Math.Max(snapshotSequenceNr, _lastSnapshotAppliedNumber);

            if (!saveIsInTime)
            {
                return false;
            }
            if (snapshotSequenceNr % _saveOnEach != 0)
            {
                return false;
            }

            return true;
        }

        public bool SnapshotsSaveInProgress => _snapshotsSaveInProgressCount > 0;

        private int _snapshotsSaveInProgressCount;

        public bool ShouldDelete(out SnapshotSelectionCriteria snapshotsToDeleteCriteria)
        {
            var maxSnapshotNumToDelete = Math.Max(_lastSavedSnapshot - _eventsToKeep, 0);
            snapshotsToDeleteCriteria = new SnapshotSelectionCriteria() {MaxSequenceNr = maxSnapshotNumToDelete};
            return true;
        }

        public void MarkSnapshotApplied(long sequenceNr)
        {
            _lastSnapshotAppliedNumber = sequenceNr;
            _lastSavedSnapshot = sequenceNr;
        }

        public void MarkSnapshotSaved(long snapshotsSequenceNumber, DateTime? saveTime = null)
        {
            _savedAt = saveTime ?? BusinessDateTime.UtcNow;
            _lastSavedSnapshot = snapshotsSequenceNumber;
            _snapshotsSaveInProgressCount--;
        }
    }
}