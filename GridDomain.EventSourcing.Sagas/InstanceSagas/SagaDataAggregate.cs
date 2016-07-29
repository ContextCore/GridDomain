using System;
using System.Collections.Generic;
using Automatonymous;
using CommonDomain.Core;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaDataAggregate<TSagaData> : AggregateBase
    {
        public TSagaData Data { get; private set; }
        //for debugging purposes
        public readonly IList<object> ReceivedMessages = new List<object>();

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
            RaiseEvent(new SagaMessageReceivedEvent<TSagaData>(Id, sagaData, @event, message));
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
            ReceivedMessages.Add(e.Message);
        }

    }
}