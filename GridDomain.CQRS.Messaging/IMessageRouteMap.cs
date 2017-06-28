using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.CQRS.Messaging
{
    public interface IMessageRouteMap
    {
        Task Register(IMessagesRouter router);
    }

    public static class MessageRouteMap
    {
        public static IMessageRouteMap New(IAggregateCommandsHandlerDescriptor descriptor, string name=null)
        {
            return new CustomRouteMap(name ?? $"map for {descriptor.GetType().Name}", r => r.RegisterAggregate(descriptor));
        }

        public static IMessageRouteMap New(ISagaDescriptor descriptor, string name=null)
        {
            return new CustomRouteMap(name ?? $"map for {descriptor.GetType().Name}", r => r.RegisterSaga(descriptor));
        }

        public static IMessageRouteMap New<TAggregateCommandsHandler>(string name=null) where TAggregateCommandsHandler:
            IAggregateCommandsHandlerDescriptor, new()
        {
            return  New(new TAggregateCommandsHandler(), name ?? typeof(TAggregateCommandsHandler).Name);
        }

        public static IMessageRouteMap New<TMessage, THandler>(Expression<Func<TMessage, Guid>> correlationPropertyExpression, string name) where THandler : IHandler<TMessage>
                                                                                                                                            where TMessage : class, IHaveSagaId, IHaveId
        {
            return new CustomRouteMap(name, r => r.RegisterHandler<TMessage, THandler>(correlationPropertyExpression));
        }
    }
}