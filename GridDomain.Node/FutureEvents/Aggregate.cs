using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.EventSourcing;

namespace GridDomain.Node.FutureEvents
{
    public class Aggregate : AggregateBase
    {
       
        #region AsyncMethods
        //keep track of all invocation to be sure only aggregate-initialized async events can be applied
        private readonly IDictionary<Guid,AsyncEventsInProgress> _asyncEventsResults = new Dictionary<Guid, AsyncEventsInProgress>();
        public readonly List<AsyncEventsInProgress> AsyncUncomittedEvents = new List<AsyncEventsInProgress>();

        public void RaiseEventAsync<TTask>(Task<TTask> eventProducer) where TTask : DomainEvent
        {
            var entityToArrayTask = eventProducer.ContinueWithSafeResultCast(@event => new DomainEvent[] { @event });
            RaiseEventAsync(entityToArrayTask);
        }

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
            Register<FutureDomainEventOccuredEvent>(Apply);
        }

        private readonly IDictionary<Guid, FutureDomainEvent> _futureEvents = new Dictionary<Guid, FutureDomainEvent>();

        public void RaiseScheduledEvent(Guid eventId)
        {
            FutureDomainEvent e;
            if (!_futureEvents.TryGetValue(eventId, out e)) return;
            RaiseEvent(new FutureDomainEventOccuredEvent(eventId));            
            RaiseEvent(e.Event);
        }

        protected void RaiseEvent(DateTime raiseTime, DomainEvent @event)
        {
            RaiseEvent(new FutureDomainEvent(Id, raiseTime, @event));
        }

        private void Apply(FutureDomainEvent e)
        {
            _futureEvents.Add(e.SourceId,e);
        }

        private void Apply(FutureDomainEventOccuredEvent e)
        {
            FutureDomainEvent evt;
            if (!_futureEvents.TryGetValue(e.SourceId, out evt)) return;
            _futureEvents.Remove(e.SourceId);
        }
        #endregion

    }

    public class NotFinishedAsyncMethodResultsRequestedException : Exception
    {
    }
}