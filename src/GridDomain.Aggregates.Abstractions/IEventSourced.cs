namespace GridDomain.Aggregates.Abstractions
{
    public interface IEventSourced
    {
        string Id { get; }
        void Apply(IDomainEvent @event);
        int Version { get; }
    }
}