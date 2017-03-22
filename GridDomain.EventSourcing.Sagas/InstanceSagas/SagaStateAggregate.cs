using System;
using System.Threading.Tasks;
using Automatonymous;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaStateAggregate<TSagaState> : Aggregate where TSagaState : ISagaState
    {
        public SagaStateAggregate(TSagaState data): this(data.Id)
        {
            RaiseEvent(new SagaCreatedEvent<TSagaState>(data, data.Id));
        }

        private SagaStateAggregate(Guid id) : base(id)
        {
            Id = id;
        }

        public TSagaState Data { get; private set; }

        public void RememberEvent(TSagaState sagaData, object message, string machineEventName)
        {
            Emit(new SagaMessageReceivedEvent<TSagaState>(Id, sagaData, machineEventName, message));
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