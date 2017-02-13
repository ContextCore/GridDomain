using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders
{
    public class FaultyCreateProjectionBuilder : IHandler<SampleAggregateCreatedEvent>
    {
        public Task Handle(SampleAggregateCreatedEvent msg)
        {
            throw new FaultyProjectionBuilderException();
        }
    }
}