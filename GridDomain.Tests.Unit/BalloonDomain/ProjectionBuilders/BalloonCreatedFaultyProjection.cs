using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders
{
    public class BalloonCreatedFaultyProjection : IHandler<BalloonCreated>
    {
        public Task Handle(BalloonCreated msg)
        {
            throw new FaultyProjectionBuilderException();
        }
    }
}