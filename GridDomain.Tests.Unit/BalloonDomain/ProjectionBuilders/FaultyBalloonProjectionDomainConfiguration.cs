using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders {
    public class FaultyBalloonProjectionDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new BalloonDependencies());

            builder.RegisterHandler<BalloonCreated, BalloonCreatedFaultyProjection>
                (c => new BalloonCreatedFaultyProjection()).AsSync();

            builder.RegisterHandler<BalloonTitleChanged, BalloonTitleChangedFaultyMessageHandler>
                (c => new BalloonTitleChangedFaultyMessageHandler(c.Publisher)).AsSync();

            builder.RegisterHandler<BalloonTitleChanged, BalloonTitleChangedOddFaultyMessageHandler>
                (c => new BalloonTitleChangedOddFaultyMessageHandler(c.Publisher)).AsSync();
        }
    }
}