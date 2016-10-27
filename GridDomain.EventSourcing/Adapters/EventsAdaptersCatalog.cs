using System;
using System.Collections.Generic;
using System.Linq;

namespace GridDomain.EventSourcing.Adapters
{
    public class EventsAdaptersCatalog
    {
        private readonly IDictionary<Type, IEventAdapter> _eventAdapterCatalog = new Dictionary<Type, IEventAdapter>();
        private readonly IDictionary<Type, IObjectAdapter> _objectAdapterCatalog = new Dictionary<Type, IObjectAdapter>();

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

        public void Register<TFrom, TTo>(IObjectAdapter<TFrom, TTo> adapter)
        {
            _objectAdapterCatalog[typeof(TFrom)] = adapter;
        }
    }
}