using System;
using System.Collections.Generic;
using System.Linq;
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


    public interface IObjectUpdateChain
    {
        object[] Update(object evt);
    }

    public class EventsAdaptersCatalog : IEventAdaptersCatalog, IObjectUpdateChain
    {
        private readonly IDictionary<Type, IEventAdapter> _eventAdapterCatalog = new Dictionary<Type, IEventAdapter>();
        private readonly List<JsonConverter> _objectAdapterCatalog = new List<JsonConverter>();

        public IReadOnlyCollection<JsonConverter> JsonConverters => _objectAdapterCatalog;

        public object[] Update(object evt)
        {
            IEventAdapter adapter;
            var processingType = evt.GetType();

            if(!_eventAdapterCatalog.TryGetValue(processingType, out adapter)) return new[]{evt};

            var updatedEvent = adapter.Convert(evt);
            return updatedEvent.SelectMany(Update).ToArray();
        }

        /// <summary>
        /// All types should form a chain 
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="adapter"></param>
        public void Register<TFrom, TTo>(IDomainEventAdapter<TFrom, TTo> adapter) where TFrom : DomainEvent where TTo : DomainEvent
        {
            _eventAdapterCatalog[typeof(TFrom)] = adapter;
        }

        public void Register<TFrom, TTo>(ObjectAdapter<TFrom, TTo> adapter)
        {
            Register((JsonConverter)adapter);
        }

        public void Register(JsonConverter converter)
        {
            _objectAdapterCatalog.Add(converter);
        }
    }
}