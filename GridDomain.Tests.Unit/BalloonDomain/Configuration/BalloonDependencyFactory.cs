using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Tests.Unit.BalloonDomain.Configuration {
    public class BalloonDependencies : AggregateDependencies<Balloon>
    {
        public BalloonDependencies() : base(() => new BalloonCommandHandler()) { }
    }
}