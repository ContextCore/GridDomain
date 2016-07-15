using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Automatonymous;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Sagas.Simplified
{
    public class SagaDataAggregate<TSagaData> : AggregateBase where TSagaData: ISagaState<State>
    {
        public TSagaData Data { get; private set; }
        //for debugging purposes
        public IList<object> ReceivedMessages = new List<object>();

        class MappedEvent
        {
                
        }

        private SagaDataAggregate(Guid id)
        {
            Id = id;
        }

        public SagaDataAggregate(Guid id, TSagaData state):this(id)
        {
            RaiseEvent(new SagaCreatedEvent<TSagaData>(state, id));
        }
        public void RememberTransition(State state, TSagaData modifiedData)
        {
            RaiseEvent(new SagaTransitionEvent<TSagaData>(Id, modifiedData, state));
        }
        public void RememberEvent(Event @event, TSagaData sagaData, object eventData = null)
        {
            RaiseEvent(new SagaEventReceivedEvent<TSagaData>(Id, sagaData, @event, eventData));
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

        public void Apply(SagaEventReceivedEvent<TSagaData> e)
        {
            Data = e.SagaDataBeforeEvent;
            ReceivedMessages.Add(e.Message);
        }

    }
}