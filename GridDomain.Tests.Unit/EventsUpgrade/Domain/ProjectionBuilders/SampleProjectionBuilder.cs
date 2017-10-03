using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain.ProjectionBuilders
{
    public class SampleProjectionBuilder : IHandler<BalanceChangedEvent_V0>, IHandler<BalanceChangedEvent_V1>
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

        public Task Handle(BalanceChangedEvent_V1 msg, IMessageMetadata metadata)
        {
            _publisher.Publish(new BalanceAggregateChangedEventNotification { AggregateId = msg.SourceId });
            return Task.CompletedTask;
        }
    }
}