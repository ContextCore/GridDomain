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
    //public class BalloonContainerConfiguration : BalloonDomainConfiguration
    //{
       
    //}

    public class BalloonDomainConfiguration : IDomainBuilderConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new BalloonDependencyFactory());
            builder.RegisterHandler<BalloonCreated, BalloonCreatedNotificator>(c => new BalloonCreatedNotificator(c.Publisher));
            builder.RegisterHandler<BalloonTitleChanged, BalloonTitleChangedNotificator>(c => new BalloonTitleChangedNotificator(c.Publisher));
            builder.RegisterHandler<BalloonCreated, BalloonCreatedFaultyProjection>();
        }
    }

    public class BalloonDependencyFactory: IAggregateDependencyFactory<Balloon>
    {
        public IAggregateCommandsHandler<Balloon> CreateCommandsHandler(string name)
        {
            return new BalloonCommandHandler();
        }

        public ISnapshotsPersistencePolicy CreatePersistencePolicy(string name)
        {
            return new SnapshotsPersistencePolicy();
        }

        public IConstructAggregates CreateFactory(string name)
        {
            return new AggregateFactory();
        }
    }
}