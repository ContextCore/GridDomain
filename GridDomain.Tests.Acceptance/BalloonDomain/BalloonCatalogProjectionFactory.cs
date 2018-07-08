using System;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Acceptance.BalloonDomain {
    public class BalloonCatalogProjectionFactory : IMessageHandlerFactory<IMessageProcessContext,BalloonTitleChanged, BalloonCatalogProjection>,
                                                   IMessageHandlerFactory<IMessageProcessContext,BalloonCreated, BalloonCatalogProjection>
    {
        private readonly Func<BalloonContext> _balloonContextProducer;
        private Lazy<BalloonCatalogProjection> _projection = new Lazy<BalloonCatalogProjection>();
        public BalloonCatalogProjectionFactory(Func<BalloonContext> balloonContextProducer)
        {
            _balloonContextProducer = balloonContextProducer;
        }
        BalloonCatalogProjection IMessageHandlerFactory<IMessageProcessContext,BalloonTitleChanged, BalloonCatalogProjection>.Create(IMessageProcessContext context)
        {
            if (!_projection.IsValueCreated)
                _projection = new Lazy<BalloonCatalogProjection>(() => new BalloonCatalogProjection(_balloonContextProducer, context.Publisher, context.Log));
            return _projection.Value;
        }

        BalloonCatalogProjection IMessageHandlerFactory<IMessageProcessContext,BalloonCreated, BalloonCatalogProjection>.Create(IMessageProcessContext context)
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