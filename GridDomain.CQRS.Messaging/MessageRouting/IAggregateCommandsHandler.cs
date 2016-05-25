using System.Collections.Generic;
using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IAggregateCommandsHandler<TAggregate>
    {
        IReadOnlyCollection<DomainEvent> Execute(TAggregate aggregate, ICommand command);
    }
}