using System;

namespace GridDomain.Configuration
{
            
    public class NoSnapshotsPersistencePolicy : ISnapshotsPersistencePolicy
    {
        class DummyOperationTracker<T> : IOperationTracker<T>
        {
            public int InProgress { get; } = 0;
            public void Start(T criteria){}
            public void Complete(T instance){}
            public void Fail(T instance){}
        }
        
        public bool ShouldSave(long snapshotSequenceNr) => false;

        public bool ShouldDelete(long lastSpanshotSaved, out SnapshotSelectionCriteria selection)
        {
            selection = SnapshotSelectionCriteria.Empty;
            return false;
        }

        IOperationTracker<SnapshotSelectionCriteria> ISnapshotsDeletePolicy.Tracking { get; } = new DummyOperationTracker<SnapshotSelectionCriteria>();

        IOperationTracker<long> ISnapshotsSavePolicy.Tracking { get; } = new DummyOperationTracker<long>();

        public bool ShouldDelete(out SnapshotSelectionCriteria selection)
        {
            selection = SnapshotSelectionCriteria.Empty;
            return false;
        }

        public void MarkSnapshotApplied(long seqNum) { }
    }
}