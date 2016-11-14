using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Tests.SampleDomain.Events;

namespace GridDomain.Tests.SampleDomain
{
    public class SampleAggregate : Aggregate
    {
        public string Value { get; private set; }

        private SampleAggregate(Guid id) : base(id)
        {
            
        }

        public SampleAggregate(Guid id, string value):this(id)
        {
            RaiseEvent(new SampleAggregateCreatedEvent(value,id));
        }

        public void ChangeState(int number)
        {
            RaiseEvent(new SampleAggregateChangedEvent(number.ToString(), Id));
        }

        public void CreateAndChangeState(string value)
        {
            RaiseEvent(new SampleAggregateCreatedEvent(value, Id));
            RaiseEvent(new SampleAggregateChangedEvent(value, Id));
        }

        public void LongExecute(int sleepMiliseconds)
        {
            Thread.Sleep(sleepMiliseconds);
            ChangeState(sleepMiliseconds);
        }

        private Task<DomainEvent[]> CreateEventsTask(int param, TimeSpan sleepTime)
        {
            var timeSpan = sleepTime;
            var eventTask = Task.Run(() =>
            {
                Thread.Sleep(timeSpan);
                return new DomainEvent[] { new SampleAggregateChangedEvent(param.ToString(), Id)};
            });
            return eventTask;
        }

        private Task<SampleAggregateChangedEvent> CreateEventTask(int param, TimeSpan sleepTime)
        {
            var timeSpan = sleepTime;
            var eventTask = Task.Run(() =>
            {
                Thread.Sleep(timeSpan);
                return new SampleAggregateChangedEvent(param.ToString(), Id);
            });
            return eventTask;
        }


        internal void ChangeStateAsync(int parameter, TimeSpan sleepTime)
        {
            var eventTask = CreateEventsTask(parameter,sleepTime);
            RaiseEventAsync(eventTask);
        }

        internal void AsyncExceptionWithOneEvent(int parameter, TimeSpan sleepTime)
        {
            var expectionTask = CreateEventTask(0, sleepTime).ContinueWith(
             t =>
             {
                 RaiseExeption();
                 return t.Result;
             });
            RaiseEventAsync(expectionTask);
        }

        private void Apply(SampleAggregateCreatedEvent e)
        {
            Id = e.SourceId;
            Value = e.Value;
        }
        
        private void Apply(SampleAggregateChangedEvent e)
        {
            Value = e.Value;
        }

        public void RaiseExeption()
        {
            throw new SampleAggregateException();
        }

        public void RaiseExeptionAsync(TimeSpan callBackTime)
        {
         var expectionTask = CreateEventsTask(0,callBackTime).ContinueWith(
             t =>
             {
                 RaiseExeption();
                 return t.Result;
             });

            RaiseEventAsync(expectionTask);
        }
    }
}