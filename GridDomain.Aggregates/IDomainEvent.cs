namespace GridDomain.Aggregates
{
    public interface IDomainEvent : ISourcedEvent, IHaveId
    {
        long Version { get; }
    }
}