using System;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing
{
    public delegate Task PersistenceDelegate(Aggregate evt);

    public interface IAggregateCommandsHandler<TAggregate> : IAggregateCommandsHandlerDescriptor
    {
        Task ExecuteAsync(TAggregate aggregate, ICommand command, PersistenceDelegate persistenceDelegate);
    }
}