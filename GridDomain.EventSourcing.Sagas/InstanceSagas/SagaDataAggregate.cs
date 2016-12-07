using System;
using System.Collections.Generic;
using Automatonymous;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing.Sagas.FutureEvents;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaDataAggregate<TSagaData> : AggregateBase
    {
        private class Snapshot : IMemento
        {
            public Snapshot(Guid id, int version, TSagaData data)
            {
                Id = id;
                Data = data;
                Version = version;
            }

            public Guid Id { get; set; }
            public int Version { get; set; }
            public TSagaData Data {get; }
        }
        protected override IMemento GetSnapshot()
        {
            return new Snapshot(Id,Version,Data);
        }

        public static SagaDataAggregate<TSagaData> FromSnapshot(IMemento m)
        {
            Snapshot s = m as Snapshot;
            if (s == null)
                throw new WrongSnapshotTypeReceivedException(m.GetType(), typeof(Snapshot));
            var sagaDataAggregate = new SagaDataAggregate<TSagaData>(s.Id, s.Data) {Version = s.Version};
            ((IAggregate)sagaDataAggregate).ClearUncommittedEvents();
            return sagaDataAggregate;    
        }

        public TSagaData Data { get; private set; }
        //for debugging purposes
        public IList<object> ReceivedMessages { get; } = new List<object>();

        private SagaDataAggregate(Guid id)
        {
            Id = id;
        }

        public SagaDataAggregate(Guid id, TSagaData data ):this(id)
        {
            RaiseEvent(new SagaCreatedEvent<TSagaData>(data, id));
        }
        public void RememberTransition(TSagaData modifiedData)
        {
            RaiseEvent(new SagaTransitionEvent<TSagaData>(Id, modifiedData));
        }

        public void RememberEvent(Event @event, TSagaData sagaData, object message)
        {
            RaiseEvent(new SagaMessageReceivedEvent<TSagaData>(Id, sagaData, @event.Name, message));
        }
        public void Apply(SagaCreatedEvent<TSagaData> e)
        {
            Data = e.State;
            Id = e.SourceId;
        }
        public void Apply(SagaTransitionEvent<TSagaData> e)
        {
            Data = e.SagaData;
        }

        public void Apply(SagaMessageReceivedEvent<TSagaData> e)
        {
            Data = e.SagaData;

            if (ReceivedMessages.Count > 10)
                ReceivedMessages.Clear();
            ReceivedMessages.Add(e.Message);
        }

    }
}