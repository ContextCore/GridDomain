namespace GridDomain.EventSourcing.CommonDomain {
    public interface ISnapshotFactory
    {
        ISnapshot GetSnapshot(IAggregate aggregate);
    }
}