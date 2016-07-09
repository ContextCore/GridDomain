using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IMessagesRouter
    {
        [Obsolete("Use RegisterHandler instead")]
        IRouteBuilder<TMessage> Route<TMessage>();

        void RegisterAggregate<TAggregate, TCommandHandler>()
            where TCommandHandler : AggregateCommandsHandler<TAggregate>, new()
            where TAggregate : AggregateBase;

        void RegisterAggregate(IAggregateCommandsHandlerDesriptor descriptor);

        void RegisterSaga(ISagaDescriptor sagaDescriptor);
        void RegisterHandler<TMessage, THandler>(string correlationField) where THandler : IHandler<TMessage>; 

        void RegisterProjectionGroup<T>(T group) where T : IProjectionGroup;
    }

    public static class MessageRouterExtensions
    {
        public static void RegisterHandler<TMessage, THandler>( this IMessagesRouter router,
            Expression<Func<TMessage, Guid>>  correlationPropertyExpression) where THandler : IHandler<TMessage>
        {
            router.RegisterHandler<TMessage,THandler>(MemberNameExtractor.GetName(correlationPropertyExpression));
        }
    }
}
