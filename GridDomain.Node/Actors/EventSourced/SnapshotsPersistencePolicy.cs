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
        public TimeSpan MaxSaveFrequency { get; }
        private readonly int _saveOnEach;
        private long _lastSavedSnapshot;
        private long _lastSnapshotAppliedNumber;
        private DateTime _lastSaveStartedAt;

        public SnapshotsPersistencePolicy(int saveOnEach = 1,
                                          int eventsToKeep = 10,
                                          TimeSpan? maxSaveFrequency = null,
                                          DateTime? savedAt = null)
        {
            MaxSaveFrequency = maxSaveFrequency ?? TimeSpan.MinValue;
            _saveOnEach = saveOnEach;
            _eventsToKeep = eventsToKeep;
            _lastSaveStartedAt = savedAt ?? DateTime.MinValue;
        }

        public void MarkSnapshotSaving(DateTime? now=null)
        {
            _snapshotsSaveInProgressCount++;
            _lastSaveStartedAt = now ?? BusinessDateTime.UtcNow;
        }

        public bool ShouldSave(long snapshotSequenceNr, DateTime? now = null)
        {
            var saveIsInTime = (now ?? BusinessDateTime.UtcNow) - _lastSaveStartedAt >= MaxSaveFrequency;
            snapshotSequenceNr = Math.Max(snapshotSequenceNr, _lastSnapshotAppliedNumber);

            if (!saveIsInTime) return false;

            return snapshotSequenceNr % _saveOnEach == 0;
        }

        public bool SnapshotsSaveInProgress => _snapshotsSaveInProgressCount > 0;

        private int _snapshotsSaveInProgressCount;

        public bool ShouldDelete(out SnapshotSelectionCriteria snapshotsToDeleteCriteria)
        {
            var maxSnapshotNumToDelete = Math.Max(_lastSavedSnapshot - _eventsToKeep, 0);
            snapshotsToDeleteCriteria = new SnapshotSelectionCriteria() {MaxSequenceNr = maxSnapshotNumToDelete};
            return maxSnapshotNumToDelete > 0;
        }

        public void MarkSnapshotApplied(long sequenceNr)
        {
            _lastSnapshotAppliedNumber = sequenceNr;
            _lastSavedSnapshot = sequenceNr;
        }


        public void MarkSnapshotSaved(long snapshotsSequenceNumber, DateTime? saveTime = null)
        {
            //save confirmations can arrive out of order 
            _lastSavedSnapshot = Math.Max(_lastSavedSnapshot,snapshotsSequenceNumber);
            _snapshotsSaveInProgressCount--;
        }
    }
}