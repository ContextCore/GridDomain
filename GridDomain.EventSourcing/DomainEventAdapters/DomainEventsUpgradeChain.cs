using System;
using System.Collections.Generic;
using CommonDomain;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public class EventAdaptersCatalog
    {
        private readonly IDictionary<Type, IEventAdapter> _adapterCatalog = new Dictionary<Type, IEventAdapter>();

        public object[] Update(object evt)
        {
            IEventAdapter adapter = null;
            var processingType = evt.GetType();
            List<object> updatedEvent =  new List<object>{evt};

            while (_adapterCatalog.TryGetValue(processingType, out adapter))
            {
                var eventsToProcess = updatedEvent.ToArray();
                processingType = adapter.Descriptor.To;

                updatedEvent.Clear();
                foreach(var ev in eventsToProcess)
                    updatedEvent.AddRange(adapter.Convert(ev));
            }

            return updatedEvent.ToArray();
        }

        /// <summary>
        /// All types should form a chain 
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="adapter"></param>
        public void Register<TFrom, TTo>(IDomainEventAdapter<TFrom, TTo> adapter) where TFrom : DomainEvent where TTo : DomainEvent
        {
            _adapterCatalog[typeof(TFrom)] = adapter;
        }
    }
}