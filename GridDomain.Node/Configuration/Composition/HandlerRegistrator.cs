using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Configuration.Composition {
    public class HandlerRegistrator<TMessage,THandler> where THandler : IHandler<TMessage>
                                                       where TMessage : class, IHaveSagaId, IHaveId
    {
        private readonly Func<IMessageProcessContext, THandler> _producer;
        private readonly IDomainBuilder _builder;

        public HandlerRegistrator(Func<IMessageProcessContext, THandler> producer, IDomainBuilder builder)
        {
            _producer = producer;
            _builder = builder;
        }

        public void AsSync()
        {
            _builder.RegisterHandler(new MessageHandlerFactory<TMessage, THandler>(_producer,() => 
                                                                                                 new CustomRouteMap(r => r.RegisterSyncHandler<TMessage,THandler>())));
        }

        public void AsParrallel()
        {
            _builder.RegisterHandler(new MessageHandlerFactory<TMessage, THandler>(_producer, () =>
                                                                                                  new CustomRouteMap(r => r.RegisterParralelHandler<TMessage, THandler>())));
        }

        public void AsFireAndForget()
        {
            _builder.RegisterHandler(new MessageHandlerFactory<TMessage, THandler>(_producer, () =>
                                                                                                  new CustomRouteMap(r => r.RegisterFireAndForgetHandler<TMessage, THandler>())));
        }
    }
}