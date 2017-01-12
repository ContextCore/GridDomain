using System;
using System.Collections.Generic;
using Automatonymous;
using CommonDomain;
using CommonDomain.Core;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaStateAggregate<TSagaState> : AggregateBase where TSagaState: ISagaState
    {
        private class Snapshot : IMemento
        {
            public Snapshot(Guid id, int version, TSagaState data)
            {
                Id = id;
                Data = data;
                Version = version;
            }

            public Guid Id { get; set; }
            public int Version { get; set; }
            public TSagaState Data {get; }
        }
        protected override IMemento GetSnapshot()
        {
            return new Snapshot(Id,Version,Data);
        }

        public static SagaStateAggregate<TSagaState> FromSnapshot(IMemento m)
        {
            Snapshot s = m as Snapshot;
            if (s == null)
                throw new WrongSnapshotTypeReceivedException(m.GetType(), typeof(Snapshot));
            var sagaDataAggregate = new SagaStateAggregate<TSagaState>(s.Data) {Version = s.Version};
            ((IAggregate)sagaDataAggregate).ClearUncommittedEvents();
            return sagaDataAggregate;    
        }

        public TSagaState Data { get; private set; }

        private SagaStateAggregate(Guid id)
        {
            Id = id;
        }

        public SagaStateAggregate(TSagaState data ):this(data.Id)
        {
            RaiseEvent(new SagaCreatedEvent<TSagaState>(data,data.Id));
        }
   
        public void RememberEvent(Event @event, TSagaState sagaData, object message)
        {
            RaiseEvent(new SagaMessageReceivedEvent<TSagaState>(Id, sagaData, @event.Name, message));
        }
        public void Apply(SagaCreatedEvent<TSagaState> e)
        {
            Data = e.State;
            Id = e.SourceId;
        }

        public void Apply(SagaMessageReceivedEvent<TSagaState> e)
        {
            Data = e.SagaData;
        }

    }
}