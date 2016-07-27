using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.EventsUpgrade.Domain.Events;

namespace GridDomain.Tests.EventsUpgrade.Domain.ProjectionBuilders
{
    public class SampleProjectionBuilder : IHandler<BalanceChangedEvent>
    {
        private readonly IPublisher _publisher;

        public SampleProjectionBuilder(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(BalanceChangedEvent msg)
        {
            _publisher.Publish(new BalanceAggregateChangedEventNotification() { AggregateId = msg.SourceId} );
        }
    }
}