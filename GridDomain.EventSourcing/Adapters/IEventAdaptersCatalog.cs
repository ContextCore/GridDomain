using Newtonsoft.Json;

namespace GridDomain.EventSourcing.Adapters
{
    public interface IEventAdaptersCatalog
    {
        /// <summary>
        /// All types should form a chain 
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="adapter"></param>
        void Register<TFrom, TTo>(IDomainEventAdapter<TFrom, TTo> adapter) where TFrom : DomainEvent where TTo : DomainEvent;

        void Register(JsonConverter converter);

        void Register<TFrom, TTo>(ObjectAdapter<TFrom, TTo> converter);
    }
}