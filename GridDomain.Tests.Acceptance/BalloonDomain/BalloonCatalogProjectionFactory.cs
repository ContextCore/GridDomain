using System;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Acceptance.BalloonDomain {
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

        public IMessageRouteMap CreateRouteMap()
        {
            return new CustomRouteMap(r => r.RegisterSyncHandler<BalloonCreated, BalloonCatalogProjection>(),
                                      r => r.RegisterSyncHandler<BalloonTitleChanged, BalloonCatalogProjection>());

        }
    }
}