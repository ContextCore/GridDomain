using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.SynchroniousCommandExecute;

namespace GridDomain.Tests.SampleDomain
{
    public class SampleProjectionBuilder : IHandler<AggregateChangedEvent>
    {
        private readonly IPublisher _publisher;

        public SampleProjectionBuilder(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(AggregateChangedEvent msg)
        {
            _publisher.Publish(new AggregateChangedEventNotification() { AggregateId = msg.SourceId} );
        }
    }
}