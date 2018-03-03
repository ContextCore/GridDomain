namespace GridDomain.Configuration {
    public interface ISnapshotsDeletePolicy
    {
        bool ShouldDelete(long lastSpanshotSaved, out SnapshotSelectionCriteria selection);
        IOperationTracker<SnapshotSelectionCriteria> Tracking { get; }
    }
}