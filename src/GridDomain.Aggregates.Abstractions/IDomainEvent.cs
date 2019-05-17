namespace GridDomain.Aggregates.Abstractions
{
    public interface IDomainEvent : ISourcedEvent, IHaveId
    {
        long Version { get; set; }
    }
}