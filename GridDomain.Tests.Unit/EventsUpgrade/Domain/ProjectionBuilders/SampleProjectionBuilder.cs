using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Routing;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain.ProjectionBuilders
{
    public class SampleProjectionBuilder : IHandler<BalanceChangedEvent_V0>
    {
        private readonly IPublisher _publisher;

        public SampleProjectionBuilder(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task Handle(BalanceChangedEvent_V0 msg,IMessageMetadata metadata)
        {
            _publisher.Publish(new BalanceAggregateChangedEventNotification {AggregateId = msg.SourceId});
            return Task.CompletedTask;
        }
    }
}