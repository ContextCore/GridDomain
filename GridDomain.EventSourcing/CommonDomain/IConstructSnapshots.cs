namespace GridDomain.EventSourcing.CommonDomain {
    public interface IConstructSnapshots
    {
        IMemento GetSnapshot(IAggregate aggregate);
    }
}