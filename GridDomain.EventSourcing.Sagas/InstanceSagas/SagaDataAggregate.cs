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

        public string CurrentStateName { get; private set; }

        public SagaDataAggregate(Guid id, TSagaData data, string stateName):this(id)
        {
            RaiseEvent(new InstanceSagaCreatedEvent<TSagaData>(data, id, stateName));
        }
        public void RememberTransition(string state, TSagaData modifiedData)
        {
            RaiseEvent(new InstanceSagaTransitionEvent<TSagaData>(Id, modifiedData, state));
        }
        public void RememberEvent(Event @event, TSagaData sagaData, object eventData = null)
        {
            RaiseEvent(new SagaMessageReceivedEvent<TSagaData>(Id, sagaData, @event, eventData));
        }

        public void Apply(InstanceSagaCreatedEvent<TSagaData> e)
        {
            Data = e.Data;
            Id = e.SourceId;
            CurrentStateName = e.StateName;
        }
        public void Apply(InstanceSagaTransitionEvent<TSagaData> e)
        {
            Data = e.SagaData;
            CurrentStateName = e.StateName;
        }

        public void Apply(SagaMessageReceivedEvent<TSagaData> e)
        {
            Data = e.SagaData;
            ReceivedMessages.Add(e.Message);
        }

    }
}