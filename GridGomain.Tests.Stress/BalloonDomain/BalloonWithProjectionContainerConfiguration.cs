using System;
using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Practices.Unity;

namespace GridGomain.Tests.Stress.BalloonDomain
{

    public class BalloonWithProjectionDomainCOnfiguration : IDomainConfiguration
    {
        private readonly string _balloonConnString;

        public BalloonWithProjectionDomainCOnfiguration(string balloonConnString)
        {
            _balloonConnString = balloonConnString;
        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new BalloonDependencyFactory());

            var options = new DbContextOptionsBuilder<ShopDbContext>().UseSqlServer(
                                                                                                                      "Server = (local); Database = Shop; Integrated Security = true; MultipleActiveResultSets = True")
                                                                                                        .Options;
            var projectionFactory = new BalloonCatalogProjectionFactory()
            builder.RegisterHandler<BalloonTitleChanged, BalloonCatalogProjection>(new BalloonMe());
            builder.RegisterHandler<BalloonCreated, BalloonCatalogProjection > (new BalloonDependencyFactory());

        }
    }

    public class BalloonCatalogProjectionFactory : IMessageHandlerFactory<BalloonTitleChanged, BalloonCatalogProjection>,
                                                   IMessageHandlerFactory<BalloonCreated, BalloonCatalogProjection>
    {
        private readonly Func<BalloonContext> _balloonContextProducer;
        private Lazy<BalloonCatalogProjection> _projection = new Lazy<BalloonCatalogProjection>();
        public BalloonCatalogProjectionFactory(Func<BalloonContext> balloonContextProducer)
        {
            _balloonContextProducer = balloonContextProducer;
        }
        BalloonCatalogProjection IMessageHandlerFactory<BalloonTitleChanged, BalloonCatalogProjection>.Create(IMessageProcessContext context)
        {
            if(!_projection.IsValueCreated)
                _projection = new Lazy<BalloonCatalogProjection>(() => new BalloonCatalogProjection(_balloonContextProducer, context.Publisher, context.Log));
            return _projection.Value;
        }

        BalloonCatalogProjection IMessageHandlerFactory<BalloonCreated, BalloonCatalogProjection>.Create(IMessageProcessContext context)
        {
            return _projection.Value;
        }
    }
  
    public class BalloonWithProjectionContainerConfiguration : IContainerConfiguration
    {
        private readonly string _balloonConnString;
        public BalloonWithProjectionContainerConfiguration(string balloonConnString)
        {
            _balloonConnString = balloonConnString;
        }

        public void Register(IUnityContainer container)
        {
            container.Register(AggregateConfiguration.New<Balloon, BalloonCommandHandler>());
            container.RegisterInstance<Func<BalloonContext>>(() => new BalloonContext(_balloonConnString));
            container.RegisterType<BalloonCatalogProjection>();
        }
    }
}