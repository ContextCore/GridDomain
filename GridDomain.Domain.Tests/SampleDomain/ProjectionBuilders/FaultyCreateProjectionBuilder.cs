using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.SampleDomain.Events;

namespace GridDomain.Tests.SampleDomain.ProjectionBuilders
{
    public class FaultyCreateProjectionBuilder : IHandler<SampleAggregateCreatedEvent>
    {
        public void Handle(SampleAggregateCreatedEvent msg)
        {
            throw new SampleAggregateException();
        }
    }
}