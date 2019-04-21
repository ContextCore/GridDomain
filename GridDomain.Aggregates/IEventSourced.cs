namespace GridDomain.Aggregates
{
    public interface IEventSourced
    {
        string Id { get; }
        void Apply(IDomainEvent @event);
        int Version { get; }
    }
}