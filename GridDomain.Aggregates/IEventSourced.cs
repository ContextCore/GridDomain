namespace GridDomain.Aggregates
{
    public interface IEventSourced
    {
        string Id { get; }
        void Apply(DomainEvent @event);
        int Version { get; }
    }

    //must be serializable


}