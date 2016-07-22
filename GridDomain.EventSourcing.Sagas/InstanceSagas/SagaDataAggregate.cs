using System;
using System.Collections.Generic;
using Automatonymous;
using CommonDomain.Core;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaDataAggregate<TSagaData> : AggregateBase// where TSagaData: ISagaState<State>
    {
        public TSagaData Data { get; private set; }
        //for debugging purposes
        public readonly IList<object> ReceivedMessages = new List<object>();

        private SagaDataAggregate(Guid id)
        {
            Id = id;
        }

        public string CurrentStateName { get; private set; }

        public SagaDataAggregate(Guid id, TSagaData state):this(id)
        {
            RaiseEvent(new SagaCreatedEvent<TSagaData>(state, id));
        }
        public void RememberTransition(string state, TSagaData modifiedData)
        {
            RaiseEvent(new SagaTransitionEvent<TSagaData>(Id, modifiedData, state));
        }
        public void RememberEvent(Event @event, TSagaData sagaData, object eventData = null)
        {
            RaiseEvent(new SagaMessageReceivedEvent<TSagaData>(Id, sagaData, @event, eventData));
        }

        public void Apply(SagaCreatedEvent<TSagaData> e)
        {
            Data = e.State;
            Id = e.SourceId;
        }
        public void Apply(SagaTransitionEvent<TSagaData> e)
        {
            Data = e.SagaData;
            CurrentStateName = e.NewMachineState;
        }

        public void Apply(SagaMessageReceivedEvent<TSagaData> e)
        {
            Data = e.SagaData;
            ReceivedMessages.Add(e.Message);
        }

    }
}