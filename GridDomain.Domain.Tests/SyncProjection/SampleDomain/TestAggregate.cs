using System;
using System.Collections.Generic;
using CommonDomain.Core;
using GridDomain.Tests.DependencyInjection;

namespace GridDomain.Tests.SyncProjection.SampleDomain
{
    public class TestAggregate : AggregateBase
    {
        private TestAggregate(Guid id)
        {
            Id = id;
        }

        public  TestAggregate(Guid id, string value):this(id)
        {
            RaiseEvent(new AggregateCreatedEvent(value,id));
        }

        public void ChangeState(int number)
        {
            RaiseEvent(new AggregateCreatedEvent(number.ToString(), Id));
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