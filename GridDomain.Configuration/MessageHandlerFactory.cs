using System;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;

namespace GridDomain.Configuration {

    public class MessageHandlerFactory<TContext, TMessage, THandler> : IMessageHandlerFactory<TContext,TMessage,THandler>
                                                             where THandler : IHandler<TMessage>
    {
        private readonly Func<TContext, THandler> _handlerCreator;
        private readonly Func<IMessageRouteMap> _mapCreator;

        public MessageHandlerFactory(Func<TContext, THandler> handlerCreator, Func<IMessageRouteMap> mapCreator)
        {
            _mapCreator = mapCreator;
            _handlerCreator = handlerCreator;
        }

        public THandler Create(TContext context)
        {
            return _handlerCreator(context);
        }

        public  IMessageRouteMap CreateRouteMap()
        {
            return _mapCreator();
        }
    }
}