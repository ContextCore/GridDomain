using System;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.BalloonDomain
{

    public class BalloonDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new BalloonDependencyFactory());
            builder.RegisterHandler<BalloonCreated, BalloonCreatedNotificator>(c => new BalloonCreatedNotificator(c.Publisher));
            builder.RegisterHandler<BalloonTitleChanged, BalloonTitleChangedNotificator>(c => new BalloonTitleChangedNotificator(c.Publisher));
            builder.RegisterHandler<BalloonCreated, BalloonCreatedFaultyProjection>();
        }
    }

    public class BalloonDependencyFactory: DefaultAggregateDependencyFactory<Balloon>
    {
        public BalloonDependencyFactory()
        {
            HandlerCreator = () => new BalloonCommandHandler();
        }
    }
}