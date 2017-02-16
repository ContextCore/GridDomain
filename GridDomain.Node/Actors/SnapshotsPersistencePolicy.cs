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
        private readonly TimeSpan? _saveMinPeriod;
        private DateTime _savedAt;

        public SnapshotsPersistencePolicy(int saveOnEach = 1,
                                          int eventsToKeep = 10,
                                          TimeSpan? maxSaveFrequency = null,
                                          DateTime? savedAt = null)
        {
            _saveMinPeriod = maxSaveFrequency ?? TimeSpan.FromSeconds(1);
            _saveOnEach = saveOnEach;
            _eventsToKeep = eventsToKeep;
            _savedAt = savedAt ?? DateTime.MinValue;
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

        public void MarkSnapshotSaved(DateTime? saveTime = null)
        {
            _savedAt = saveTime ?? BusinessDateTime.UtcNow;
        }

        public bool ShouldSave(DateTime? now)
        {
            bool saveIsInTime = (now ?? BusinessDateTime.UtcNow) - _savedAt >= _saveMinPeriod;
            return _lastSequenceNumber%_saveOnEach == 0 && saveIsInTime;
        }
    }
}