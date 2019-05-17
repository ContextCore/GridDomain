namespace GridDomain.Aggregates.Abstractions
{
    public interface IAggregateConfiguration<TAggregate> where TAggregate : IAggregate
    {
        IAggregateFactory<TAggregate> AggregateFactory { get; }
        IAggregateSettings Settings { get; }
    }
}