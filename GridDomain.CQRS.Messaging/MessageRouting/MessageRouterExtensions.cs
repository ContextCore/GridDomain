using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public static class MessageRouterExtensions
    {
        //TODO: add version without correlation property
        public static Task RegisterHandler<TMessage, THandler>(this IMessagesRouter router,
            Expression<Func<TMessage, Guid>>  correlationPropertyExpression = null) where THandler : IHandler<TMessage>
        {
            return router.RegisterHandler<TMessage,THandler>(MemberNameExtractor.GetName(correlationPropertyExpression));
        }

        public static Task RegisterSaga<TSaga,TData>(this IMessagesRouter router,params Type[] startMessages) 
            where TSaga : Saga<TData>, new()
            where TData : class, ISagaState
        {
            return router.RegisterSaga(new TSaga().CreateDescriptor<TSaga,TData>(startMessages), typeof(TSaga).Name);
        }
    }
}