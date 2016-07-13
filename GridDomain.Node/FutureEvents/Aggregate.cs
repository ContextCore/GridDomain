using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;

namespace GridDomain.Node.FutureEvents
{ 
    public class Aggregate : AggregateBase
    {
       
        #region AsyncMethods
        //keep track of all invocation to be sure only aggregate-initialized async events can be applied
        private readonly IDictionary<Guid,AsyncEventsInProgress> _asyncEventsResults = new Dictionary<Guid, AsyncEventsInProgress>();
        public readonly List<AsyncEventsInProgress> AsyncUncomittedEvents = new List<AsyncEventsInProgress>();

        //protected void RaiseEventAsync<TDomainEvent>(Task<TDomainEvent> eventProducer) where TDomainEvent : DomainEvent
        //{
        //    RaiseEventAsync(eventProducer.ContinueWith(t => 
        //                                new DomainEvent[] {t.Result},
        //                                  TaskContinuationOptions.OnlyOnRanToCompletion));
        //}

        protected void RaiseEventAsync(Task<DomainEvent[]> eventProducer)
        {
            var asyncMethodStarted = new AsyncEventsInProgress(eventProducer,Guid.NewGuid());
            AsyncUncomittedEvents.Add(asyncMethodStarted);
            _asyncEventsResults.Add(asyncMethodStarted.InvocationId, asyncMethodStarted);
        }

        public void FinishAsyncExecution(Guid invocationId)
        {
            AsyncEventsInProgress eventsInProgress;

            if (!_asyncEventsResults.TryGetValue(invocationId, out eventsInProgress)) return;
            if (!eventsInProgress.ResultProducer.IsCompleted)
                throw new NotFinishedAsyncMethodResultsRequestedException();
            _asyncEventsResults.Remove(invocationId);

            foreach (var @event in eventsInProgress.ResultProducer.Result)
                RaiseEvent(@event);
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

    public class NotFinishedAsyncMethodResultsRequestedException : Exception
    {
    }
}