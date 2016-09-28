using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.SampleDomain.Events;

namespace GridDomain.Tests.SampleDomain.ProjectionBuilders
{
    public class CreateProjectionBuilder : IHandler<SampleAggregateCreatedEvent>
    {
        private readonly IPublisher _publisher;

        public CreateProjectionBuilder(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(SampleAggregateCreatedEvent msg)
        {
            throw new SampleAggregateException();

            _publisher.Publish(new AggregateCreatedEventNotification() { AggregateId = msg.SourceId} );
        }
    }
}