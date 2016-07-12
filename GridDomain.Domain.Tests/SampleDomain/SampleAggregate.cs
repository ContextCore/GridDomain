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

        public void ChangeStateAsync(int param)
        {
            var random = new Random();
            var millisecandsToWait = (int)random.NextDouble()*1000;
            var eventTask = Task.Run(() =>
            {
                Thread.Sleep(millisecandsToWait);
                return new AggregateChangedEvent(param.ToString(), Id);
            });

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
    }

    public class SampleAggregateException : Exception
    {
    }
}