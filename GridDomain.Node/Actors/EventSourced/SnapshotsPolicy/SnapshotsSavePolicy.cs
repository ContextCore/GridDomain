using System;
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
}