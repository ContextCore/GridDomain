using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{
    public interface IAggregateCommandsHandler<TAggregate> : IAggregateCommandsHandlerDescriptor where TAggregate: IAggregate
    {
        Task<TAggregate> ExecuteAsync(TAggregate aggregate, ICommand command, PersistenceDelegate persistenceDelegate);
    }
}