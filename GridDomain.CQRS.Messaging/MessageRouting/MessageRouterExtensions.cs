using System;
using System.Linq.Expressions;
using GridDomain.Common;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public static class MessageRouterExtensions
    {
        public static void RegisterHandler<TMessage, THandler>( this IMessagesRouter router,
            Expression<Func<TMessage, Guid>>  correlationPropertyExpression) where THandler : IHandler<TMessage>
        {
            router.RegisterHandler<TMessage,THandler>(MemberNameExtractor.GetName(correlationPropertyExpression));
        }
    }
}