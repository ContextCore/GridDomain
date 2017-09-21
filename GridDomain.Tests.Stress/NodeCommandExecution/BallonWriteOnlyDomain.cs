using Akka.Event;
using GridDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;

namespace GridDomain.Tests.Stress.NodeCommandExecution {


    public class BallonWriteOnlyDomain : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new BalloonDependencyFactory());
        }
    }
}