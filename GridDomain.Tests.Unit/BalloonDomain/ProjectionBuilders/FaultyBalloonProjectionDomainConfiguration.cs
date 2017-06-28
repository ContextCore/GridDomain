using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders {
    public class FaultyBalloonProjectionDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new BalloonDependencyFactory());

            builder.RegisterHandler(new DefaultMessageHandlerFactory<BalloonCreated, BalloonCreatedFaultyProjection>
                (c => new BalloonCreatedFaultyProjection(), m => m.SourceId));

            builder.RegisterHandler(new DefaultMessageHandlerFactory<BalloonTitleChanged, BalloonTitleChangedFaultyMessageHandler>
                (c => new BalloonTitleChangedFaultyMessageHandler(c.Publisher), m => m.SourceId));

            builder.RegisterHandler(new DefaultMessageHandlerFactory<BalloonTitleChanged, BalloonTitleChangedOddFaultyMessageHandler>
                (c => new BalloonTitleChangedOddFaultyMessageHandler(c.Publisher), m => m.SourceId));
        }
    }
}