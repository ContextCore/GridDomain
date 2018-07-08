using System;
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
}