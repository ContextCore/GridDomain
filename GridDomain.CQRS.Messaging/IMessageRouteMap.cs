using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging
{
    public interface IMessageRouteMap
    {
        Task Register(IMessagesRouter router);

        //for diagnose purposes
        string Name { get; }
    }

    public static class MessageRouteMap
    {
        public static IMessageRouteMap New(IAggregateCommandsHandlerDescriptor descriptor, string name)
        {
            return new CustomRouteMap(name, r => r.RegisterAggregate(descriptor));
        }

        public static IMessageRouteMap New<TMessage, THandler>(Expression<Func<TMessage, Guid>> correlationPropertyExpression, string name) where THandler : IHandler<TMessage>
                                                                                                                                            where TMessage : class, IHaveSagaId, IHaveId
        {
            return new CustomRouteMap(name, r => r.RegisterHandler<TMessage, THandler>(correlationPropertyExpression));
        }
    }
}