using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Configuration.Composition {
    public static class DefaultAggregateDependencyFactory
    {
        public static DefaultAggregateDependencyFactory<TAggregate> New<TAggregate>(IAggregateCommandsHandler<TAggregate> handler, IMessageRouteMap mapProducer=null) where TAggregate : Aggregate
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var map = mapProducer ?? MessageRouteMap.New(handler);

            return new DefaultAggregateDependencyFactory<TAggregate>(() => handler,() => map);
        }
        
        public static DefaultAggregateDependencyFactory<TAggregate> New<TAggregate, TAggregateCommandsHandler>()
            where TAggregate : Aggregate
            where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>, new()
        {
            return New(new TAggregateCommandsHandler());
        }
    }
}