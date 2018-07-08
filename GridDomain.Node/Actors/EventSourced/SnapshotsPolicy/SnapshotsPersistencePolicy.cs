using System;
using GridDomain.Configuration;
using GridDomain.Configuration.SnapshotPolicies;

namespace GridDomain.Node.Actors.EventSourced.SnapshotsPolicy {
    
    public class SnapshotsPersistencePolicy : ISnapshotsPersistencePolicy
    {
        private readonly SnapshotsSavePolicy _savePolicy;

        public SnapshotsPersistencePolicy(int saveOnEach = 1,
                                           TimeSpan? maxSaveFrequency = null,
                                           int eventsToKeep = 10,
                                           TimeSpan? eventAgeToKeep = null)
        {
                 DeletePolicy = new SnapshotsDeletePolicy(eventsToKeep,eventAgeToKeep);
                 _savePolicy = new SnapshotsSavePolicy(saveOnEach,maxSaveFrequency);
        }

        public ISnapshotsSavePolicy SavePolicy => _savePolicy;
        public ISnapshotsDeletePolicy DeletePolicy { get; }
        
        public bool ShouldDelete(int lastSpanshotSaved, out SnapshotSelectionCriteria selection)
        {
            return DeletePolicy.ShouldDelete(lastSpanshotSaved, out selection);
        }

        public bool ShouldSave(long snapshotSequenceNr)
        {
            return SavePolicy.ShouldSave(snapshotSequenceNr);
        }

        public bool ShouldDelete(long lastSpanshotSaved, out SnapshotSelectionCriteria selection)
        {
            return DeletePolicy.ShouldDelete(lastSpanshotSaved, out selection);
        }

        IOperationTracker<long> ISnapshotsSavePolicy.Tracking => SavePolicy.Tracking;

        public IOperationTracker<SnapshotSelectionCriteria> Tracking => DeletePolicy.Tracking;

        public bool ShouldDelete(out SnapshotSelectionCriteria selection)
        {
            return DeletePolicy.ShouldDelete(_savePolicy.Traker.GreatestSavedNumber, out selection);
        }

        public void MarkSnapshotApplied(long seqNum)
        {
            _savePolicy.Traker.GreatestSavedNumber = seqNum;
        }
    }
}