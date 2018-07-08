using System;
using System.Collections.Generic;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.SnapshotPolicies;

namespace GridDomain.Node.Actors.EventSourced.SnapshotsPolicy {
    public class SnapshotsSavePolicy : ISnapshotsSavePolicy
    {
        private int _saveOnEach;
        public SnapshotsSaveTracker Traker { get; }
        public TimeSpan MaxSaveFrequency { get; }
        public SnapshotsSavePolicy(int saveOnEach = 1, TimeSpan? maxSaveFrequency=null)
        {
            _saveOnEach = saveOnEach;
            MaxSaveFrequency = maxSaveFrequency ?? TimeSpan.Zero;
            //to allow first event save without knowing last persisted seq number;
            Traker = new SnapshotsSaveTracker(-saveOnEach);
        }

        public bool ShouldSave(long snapshotSequenceNr)
        {
            return CanSaveByTime() && CanSaveByNumber(snapshotSequenceNr);
        }

        private bool CanSaveByNumber(long snapshotSequenceNr)
        {
            return snapshotSequenceNr - Math.Max(Traker.GreatestSavedNumber,Traker.GreatestSaveAttemptNumber) >= _saveOnEach;
        }

        private bool CanSaveByTime()
        {
            return BusinessDateTime.UtcNow - Max(Traker.LastSaveTime, Traker.LastSaveAttemptTime) >= MaxSaveFrequency;
        }

        private DateTime Max(DateTime A, DateTime B)
        {
            return A.Ticks > B.Ticks ? A : B;
        }

        public IOperationTracker<long> Tracking => Traker;
    }


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
            _active.RemoveWhere(s => s <= seqNumber);
            GreatestSavedNumber = Math.Max(seqNumber, GreatestSavedNumber);
            LastSaveTime = BusinessDateTime.UtcNow;
        }

        public void Fail(long seqNumber)
        {
            _active.Remove(seqNumber);
            LastSaveAttemptTime = LastSaveTime;
            GreatestSaveAttemptNumber = GreatestSavedNumber;
        }
    }
}