using System;
using System.Collections.Generic;
using GridDomain.Common;
using GridDomain.Configuration.SnapshotPolicies;

namespace GridDomain.Node.Actors.EventSourced.SnapshotsPolicy {
    public class SnapshotsSaveTracker : IOperationTracker<long>
    {
        public int InProgress => _active.Count;
        private readonly HashSet<long> _active = new HashSet<long>();
        
        public DateTime LastSaveTime { get; set; }
        public DateTime LastSaveAttemptTime { get; private set; }
        
        public long GreatestSavedNumber { get; internal set; }
        public long GreatestSaveAttemptNumber { get; private set; }

        public SnapshotsSaveTracker(long greatestSaveNum)
        {
            //to allow first event save without knowing last seq number;
            GreatestSavedNumber = greatestSaveNum;
            GreatestSaveAttemptNumber = greatestSaveNum;
        }
        
        public void Start(long criteria)
        {
            LastSaveAttemptTime = BusinessDateTime.UtcNow;
            GreatestSaveAttemptNumber = criteria;
            
            _active.Add(criteria);
        }

        public void Complete(long seqNumber)
        {
            LastSaveTime = BusinessDateTime.UtcNow;
            _active.RemoveWhere(s => s <= seqNumber);
            GreatestSavedNumber = Math.Max(seqNumber, GreatestSavedNumber);
        }

        public void Fail(long seqNumber)
        {
            _active.Remove(seqNumber);
            LastSaveAttemptTime = LastSaveTime;
            GreatestSaveAttemptNumber = GreatestSavedNumber;
        }
    }
}