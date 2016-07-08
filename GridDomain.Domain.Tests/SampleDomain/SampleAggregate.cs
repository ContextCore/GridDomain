using System;
using System.Threading;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.SynchroniousCommandExecute;

namespace GridDomain.Tests.SyncProjection.SampleDomain
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

    public class SampleProjectionBuilder : IHandler<AggregateChangedEvent>
    {
        private readonly IPublisher _publisher;

        public SampleProjectionBuilder(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(AggregateChangedEvent msg)
        {
            _publisher.Publish(new AggregateChangedEventNotification() { AggregateId = msg.SourceId} );
        }
    }
}