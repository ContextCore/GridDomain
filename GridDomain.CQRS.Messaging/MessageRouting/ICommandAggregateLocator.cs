using System;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface ICommandAggregateLocator<TAggregate>
    {
        Guid GetAggregateId(ICommand command);
    }
}