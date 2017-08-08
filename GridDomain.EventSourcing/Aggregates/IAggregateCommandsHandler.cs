using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Aggregates
{
    public interface IAggregateCommandsHandler<TAggregate> : IAggregateCommandsHandlerDescriptor
    {
        Task ExecuteAsync(TAggregate aggregate, ICommand command, PersistenceDelegate persistenceDelegate);
    }
}