using Akka.Actor;
using Autofac;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.EventHandlers.Akka {

    
    public class EventHandlersExtension : IExtension
    {
        private readonly IContainer _container;

        public EventHandlersExtension(IContainer container)
        {
            _container = container;
        }
     
        public T GetHandler<T>()
        {
            _container.TryResolve(typeof(T), out var handler);
            return (T)handler;
        }
    }
}