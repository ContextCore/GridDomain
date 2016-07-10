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


    public class SampleAggregate : AggregateBase
    {
        private SampleAggregate(Guid id)
        {
            Id = id;
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
            var millisecandsToWait = random.Next()*1000;
            var eventTask = Task.Run(() =>
            {
                Thread.Sleep(millisecandsToWait);
                return new AggregateChangedEvent(param.ToString(), Id);
            });

            RaiseEvent(eventTask);
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
    }
}