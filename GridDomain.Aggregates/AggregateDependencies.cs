namespace GridDomain.Aggregates
{
    public class AggregateDependencies<TAggregate>:IAggregateDependencies<TAggregate> where TAggregate : IAggregate
    {
        public AggregateDependencies(IAggregateFactory<TAggregate> factory=null,IAggregateConfiguration configuration = null)
        {
              AggregateFactory = factory ?? AggregateFactory;
              Configuration = configuration ?? Configuration;
        }
        public IAggregateFactory<TAggregate> AggregateFactory { get; set; } = GridDomain.Aggregates.AggregateFactory.For<TAggregate>();
        public IAggregateConfiguration Configuration { get; set; } = new AggregateConfiguration();
    }
}