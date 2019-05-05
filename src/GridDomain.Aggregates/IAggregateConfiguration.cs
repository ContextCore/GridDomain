using System.Threading.Tasks;

namespace GridDomain.Aggregates
{
    public interface IAggregateConfiguration<TAggregate> where TAggregate : IAggregate
    {
        IAggregateFactory<TAggregate> AggregateFactory { get; }
        IAggregateSettings Settings { get; }
    }
}