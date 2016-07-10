using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;

namespace GridDomain.Node.FutureEvents
{

    public interface IAsyncEventsProducer
    {
        void ApplyAsyncMethodResults(Guid invocationId, DomainEvent[] result);
        ICollection<AsyncMethodStarted> AsyncMethodsStarted { get; }
        void ClearAsyncMethodStartedInfo();
    }

    public class Aggregate : AggregateBase
    {
       
        #region AsyncMethods

        public readonly List<AsyncMethodStarted> AsyncMethodsStarted = new List<AsyncMethodStarted>();


        protected void RaiseEventAsync<TDomainEvent>(Task<TDomainEvent> eventProducer) where TDomainEvent : DomainEvent
        {
            RaiseEventAsync(eventProducer.ContinueWith(t => new DomainEvent[] {t.Result}));
        }

        protected void RaiseEventAsync(Task<DomainEvent[]> eventProducer)
        {
            //var domainEventApplyToAggregateTask = 
            //eventProducer.ContinueWith(t => 
            //{
            //    //TODO: move RaiseEvent in sync command to call from infrastructure
            //    foreach (var ev in t.Result)
            //        RaiseEvent(ev);
            //    return t.Result;
            //});
            AsyncMethodsStarted.Add(new AsyncMethodStarted(eventProducer));
        }

   
        #endregion
        #region FutureEvents
        protected Aggregate(Guid id)
        {
            Id = id;
            Register<FutureDomainEvent>(Apply);
        }

        private readonly IDictionary<Guid, FutureDomainEvent> _futureEvents = new Dictionary<Guid, FutureDomainEvent>();

        public void RaiseScheduledEvent(Guid eventId)
        {
            FutureDomainEvent e;
            if (!_futureEvents.TryGetValue(eventId, out e)) return;
            RaiseEvent(e.Event);
        }

        private void Apply(FutureDomainEvent e)
        {
            _futureEvents.Add(e.SourceId,e);
        }
        #endregion
    }
}