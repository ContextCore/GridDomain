using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.SampleDomain.Events;

namespace GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders
{
    public class FaultyCreateProjectionBuilder : IHandler<SampleAggregateCreatedEvent>
    {
        public Task Handle(SampleAggregateCreatedEvent msg)
        {
            throw new SampleAggregateException();
        }
    }
}