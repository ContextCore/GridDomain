using System;
using System.Collections.Generic;
using GridDomain.Configuration;
using GridDomain.Configuration.SnapshotPolicies;
using SnapshotSelectionCriteria = GridDomain.Configuration.SnapshotPolicies.SnapshotSelectionCriteria;

namespace GridDomain.Node.Actors.EventSourced.SnapshotsPolicy {
    public class SnapshotsDeletePolicy : ISnapshotsDeletePolicy
    {
        private readonly SnapshotsDeleteTracker _tracker;
        private int _eventsToKeep;
        public TimeSpan MaxSnapshotAge { get; }

        public SnapshotsDeletePolicy(int eventsToKeep = 1, TimeSpan? snapshotsAgeToKeep = null)
        {
            MaxSnapshotAge = snapshotsAgeToKeep ?? TimeSpan.Zero;
            _eventsToKeep = eventsToKeep;
            _tracker = new SnapshotsDeleteTracker();
        }
        public bool ShouldDelete(long lastSpanshotSaved, out SnapshotSelectionCriteria selection)
        {
            var seqNumToKeep = lastSpanshotSaved - _eventsToKeep;
            var seqNumDeleted = Math.Max(_tracker.GreatesDeletedNumber, _tracker.GreatestDeleteAttemtedNumber);

            if (seqNumToKeep > seqNumDeleted)
            {
                selection = new SnapshotSelectionCriteria(seqNumToKeep);
                return true;
            }
            
            selection = SnapshotSelectionCriteria.Empty;
            return false;
        }

   

        public IOperationTracker<SnapshotSelectionCriteria> Tracking => _tracker;
    }
    
    class SnapshotsDeleteTracker : IOperationTracker<GridDomain.Configuration.SnapshotPolicies.SnapshotSelectionCriteria>
    {
        public int InProgress => _active.Count;
        private readonly HashSet<SnapshotSelectionCriteria> _active = new HashSet<SnapshotSelectionCriteria>();
        public long GreatesDeletedNumber { get; private set; }
        public long GreatestDeleteAttemtedNumber { get; private set; }
        
        public SnapshotsDeleteTracker()
        {
            //to allow first event save without knowing last seq number;
           // GreatestSavedNumber = greatestSaveNum;
           // GreatestSaveAttemptNumber = greatestSaveNum;
        }
        
        public void Start(SnapshotSelectionCriteria criteria)
        {
           // LastSaveAttemptTime = BusinessDateTime.UtcNow;
           GreatestDeleteAttemtedNumber = criteria.MaxSequenceNr;
            _active.Add(criteria);
        }

        public void Complete(SnapshotSelectionCriteria criteria)
        {
            _active.RemoveWhere(c => c.MaxSequenceNr <= criteria.MaxSequenceNr && c.MaxTimeStamp <= criteria.MaxTimeStamp);
            GreatesDeletedNumber = criteria.MaxSequenceNr;
        }

        public void Fail(SnapshotSelectionCriteria criteria)
        {
            _active.Remove(criteria);
            GreatestDeleteAttemtedNumber = GreatesDeletedNumber;
        }
    }
}