namespace GridDomain.Configuration {
    public interface ISnapshotsSavePolicy 
    {
        bool ShouldSave(long snapshotSequenceNr);
        IOperationTracker<long> Tracking { get; }
    }
}