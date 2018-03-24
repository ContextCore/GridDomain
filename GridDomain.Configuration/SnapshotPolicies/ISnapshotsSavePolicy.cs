namespace GridDomain.Configuration.SnapshotPolicies {
    public interface ISnapshotsSavePolicy 
    {
        bool ShouldSave(long snapshotSequenceNr);
        IOperationTracker<long> Tracking { get; }
    }
}