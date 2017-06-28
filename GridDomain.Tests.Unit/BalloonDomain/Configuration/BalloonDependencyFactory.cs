using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Tests.Unit.BalloonDomain.Configuration {
    public class BalloonDependencyFactory : DefaultAggregateDependencyFactory<Balloon>
    {
        public BalloonDependencyFactory() : base(() => new BalloonCommandHandler()) { }
    }
}