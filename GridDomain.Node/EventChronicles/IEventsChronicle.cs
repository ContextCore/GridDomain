using System;
using CommonDomain.Core;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.EventChronicles
{
    public interface IEventsChronicle
    {
        IMessagesRouter Router { get; }

        void Replay<TAggregate>(Guid aggregateId, Predicate<object> eventFilter) where TAggregate : AggregateBase;
    }
}