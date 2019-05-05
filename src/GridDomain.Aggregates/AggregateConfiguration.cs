namespace GridDomain.Aggregates
{
    public class AggregateConfiguration<TAggregate>:IAggregateConfiguration<TAggregate> where TAggregate : IAggregate
    {
        public AggregateConfiguration(IAggregateFactory<TAggregate> factory=null,IAggregateSettings settings = null)
        {
              AggregateFactory = factory ?? AggregateFactory;
              Settings = settings ?? Settings;
        }
        public IAggregateFactory<TAggregate> AggregateFactory { get; set; } = GridDomain.Aggregates.AggregateFactory.For<TAggregate>();
        public IAggregateSettings Settings { get; set; } = new AggregateSettings();
        public ICommandHandler<ICommand> BuildCommandHandler(ICommandHandler<ICommand> handler) => handler;
    }
}