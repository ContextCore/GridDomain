namespace GridDomain.Aggregates
{
    public class AggregateDependencies<TAggregate>:IAggregateDependencies<TAggregate> where TAggregate : IAggregate
    {
        public AggregateDependencies(IAggregateFactory<TAggregate> factory=null)
        {
            if(factory!= null)
                AggregateFactory = factory;
        }
        public IAggregateFactory<TAggregate> AggregateFactory { get; } = GridDomain.Aggregates.AggregateFactory.For<TAggregate>();
        public IAggregateConfiguration Configuration { get;  } = new AggregateConfiguration();
    }
}