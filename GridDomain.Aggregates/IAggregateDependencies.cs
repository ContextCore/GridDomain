namespace GridDomain.Aggregates
{
    public interface IAggregateDependencies<TAggregate> where TAggregate : IAggregate
    {
        IAggregateFactory<TAggregate> AggregateFactory { get; }
        IAggregateConfiguration Configuration { get; }
    }
}