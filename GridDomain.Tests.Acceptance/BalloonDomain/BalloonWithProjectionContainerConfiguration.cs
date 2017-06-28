using System;
using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonWithProjectionDomainConfiguration : IDomainConfiguration
    {
        private readonly string _balloonConnString;

        public BalloonWithProjectionDomainConfiguration(string balloonConnString)
        {
            _balloonConnString = balloonConnString;
        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new BalloonDependencyFactory());

            var options = new DbContextOptionsBuilder<BalloonContext>().UseSqlServer(_balloonConnString).Options;
            BalloonContext BalloonContextProducer() => new BalloonContext(options);

            var projectionFactory = new BalloonCatalogProjectionFactory(BalloonContextProducer);
            builder.RegisterHandler<BalloonTitleChanged, BalloonCatalogProjection>(projectionFactory);
            builder.RegisterHandler<BalloonCreated, BalloonCatalogProjection>(projectionFactory);
            builder.RegisterAggregate(new BalloonDependencyFactory());
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
            if (!_projection.IsValueCreated)
                _projection = new Lazy<BalloonCatalogProjection>(() => new BalloonCatalogProjection(_balloonContextProducer, context.Publisher, context.Log));
            return _projection.Value;
        }

        BalloonCatalogProjection IMessageHandlerFactory<BalloonCreated, BalloonCatalogProjection>.Create(IMessageProcessContext context)
        {
            return _projection.Value;
        }
    }
}