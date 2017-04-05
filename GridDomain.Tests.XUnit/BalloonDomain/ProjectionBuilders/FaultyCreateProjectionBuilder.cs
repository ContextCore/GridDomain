using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.BalloonDomain.Events;

namespace GridDomain.Tests.XUnit.BalloonDomain.ProjectionBuilders
{
    public class FaultyCreateProjectionBuilder : IHandler<BalloonCreated>
    {
        public Task Handle(BalloonCreated msg)
        {
            throw new FaultyProjectionBuilderException();
        }
    }
}