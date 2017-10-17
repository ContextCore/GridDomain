using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.ProcessManagers.DomainBind;

namespace GridDomain.Configuration.MessageRouting
{
    public static class MessageRouteMap
    {
        public static IMessageRouteMap New(IAggregateCommandsHandlerDescriptor descriptor, string name = null)
        {
            return new CustomRouteMap(name ?? $"map for {descriptor.GetType().Name}",
                r => r.RegisterAggregate(descriptor));
        }

        public static IMessageRouteMap New(IProcessDescriptor descriptor, string name = null)
        {
            return new CustomRouteMap(name ?? $"map for {descriptor.GetType().Name}", r => r.RegisterProcess(descriptor));
        }

        public static IMessageRouteMap New<TAggregateCommandsHandler>(string name = null)
            where TAggregateCommandsHandler :
            IAggregateCommandsHandlerDescriptor, new()
        {
            return New(new TAggregateCommandsHandler(), name ?? typeof(TAggregateCommandsHandler).Name);
        }

        public static IMessageRouteMap NewSync<TMessage, THandler>(string name) where THandler : IHandler<TMessage>
                                                                            where TMessage : class, IHaveProcessId, IHaveId
        {
            return new CustomRouteMap(name, r => r.RegisterSyncHandler<TMessage, THandler>());
        }
    }
}