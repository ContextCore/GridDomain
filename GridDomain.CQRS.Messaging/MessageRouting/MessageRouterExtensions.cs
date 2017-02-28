using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public static class MessageRouterExtensions
    {
        //TODO: add version without correlation property
        public static Task RegisterHandler<TMessage, THandler>(this IMessagesRouter router,
                                                               Expression<Func<TMessage, Guid>> correlationPropertyExpression
                                                                   = null) where THandler : IHandler<TMessage>
            where TMessage : DomainEvent
        {
            return router.RegisterHandler<TMessage, THandler>(MemberNameExtractor.GetName(correlationPropertyExpression));
        }

        public static Task RegisterAggregate<TAggregate, TCommandHandler>(this IMessagesRouter router)
            where TAggregate : AggregateBase where TCommandHandler : AggregateCommandsHandler<TAggregate>, new()
        {
            return RegisterAggregate(router, new TCommandHandler());
        }

        public static Task RegisterAggregate<TAggregate>(this IMessagesRouter router,
                                                         AggregateCommandsHandler<TAggregate> handler)
            where TAggregate : AggregateBase
        {
            var descriptor = new AggregateCommandsHandlerDescriptor<TAggregate>();
            foreach (var info in handler.RegisteredCommands) descriptor.RegisterCommand(info.CommandType);

            return router.RegisterAggregate(descriptor);
        }
    }
}