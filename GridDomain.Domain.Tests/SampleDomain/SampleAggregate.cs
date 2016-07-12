using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.Node.FutureEvents;

namespace GridDomain.Tests.SampleDomain
{
    public class SampleAggregate : Aggregate
    {
        private SampleAggregate(Guid id) : base(id)
        {
            
        }

        public  SampleAggregate(Guid id, string value):this(id)
        {
            RaiseEvent(new AggregateCreatedEvent(value,id));
        }

        public void ChangeState(int number)
        {
            RaiseEvent(new AggregateChangedEvent(number.ToString(), Id));
        }

        public void LongExecute(int number)
        {
            Thread.Sleep(1000);
            ChangeState(number);
        }

        private Task<AggregateChangedEvent> CreateEventTask(int param, TimeSpan sleepTime)
        {
            var timeSpan = sleepTime;
            var eventTask = Task.Run(() =>
            {
                Thread.Sleep(timeSpan);
                return new AggregateChangedEvent(param.ToString(), Id);
            });
            return eventTask;
        }

        internal void ChangeStateAsync(int parameter, TimeSpan sleepTime)
        {
            var eventTask = CreateEventTask(parameter,sleepTime);
            RaiseEventAsync(eventTask);
        }

        private void Apply(AggregateCreatedEvent e)
        {
            Id = e.SourceId;
            Value = e.Value;
        }
        
        private void Apply(AggregateChangedEvent e)
        {
            Value = e.Value;
        }

        public string Value;

        public void RaiseExeption()
        {
            throw new SampleAggregateException();
        }

        public void RaiseExeptionAsync(TimeSpan callBackTime)
        {
            CreateEventTask(0,callBackTime).ContinueWith(t => RaiseExeption());
        }
    }
}