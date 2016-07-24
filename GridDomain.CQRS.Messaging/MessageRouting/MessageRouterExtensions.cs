using System;
using System.Linq.Expressions;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public static class MessageRouterExtensions
    {
        public static void RegisterHandler<TMessage, THandler>( this IMessagesRouter router,
            Expression<Func<TMessage, Guid>>  correlationPropertyExpression) where THandler : IHandler<TMessage>
        {
            router.RegisterHandler<TMessage,THandler>(MemberNameExtractor.GetName(correlationPropertyExpression));
        }

        public static void RegisterSaga<TSaga,TData>(this IMessagesRouter router) 
            where TSaga : Saga<TData>, new()
            where TData : class, ISagaState
        {
            router.RegisterSaga(new TSaga().GetDescriptor(), typeof(TSaga).Name);
        }
    }
}