using System;
using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;

namespace GridDomain.Tests.Unit.BalloonDomain.Configuration
{
    
    public class BalloonDomainConfiguration : IDomainConfiguration
    {
        public BalloonDependencyFactory BalloonDependencyFactory { get; }

        public BalloonDomainConfiguration()
        {
            BalloonDependencyFactory = new BalloonDependencyFactory();
        }

        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(BalloonDependencyFactory);
            builder.RegisterHandler<BalloonCreated, BalloonCreatedNotificator> (c => new BalloonCreatedNotificator(c.Publisher)).AsSync();
            builder.RegisterHandler<BalloonTitleChanged, BalloonTitleChangedNotificator>(c => new BalloonTitleChangedNotificator(c.Publisher)).AsSync();
        }
    }
}