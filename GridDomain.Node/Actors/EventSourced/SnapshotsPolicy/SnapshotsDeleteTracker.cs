using System.Collections.Generic;
using GridDomain.Configuration.SnapshotPolicies;

namespace GridDomain.Node.Actors.EventSourced.SnapshotsPolicy {
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