using System;
using Automatonymous;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaStateAggregate<TSagaState> : Aggregate where TSagaState: ISagaState
    {
        public TSagaState Data { get; private set; }

        private SagaStateAggregate(Guid id):base(id)
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