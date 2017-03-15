using System;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaStateCommandHandler<TSagaState> : AggregateCommandsHandler<SagaStateAggregate<TSagaState>> where TSagaState : ISagaState
    {
        public SagaStateCommandHandler()
        {
            Map<SaveStateCommand<TSagaState>>((c, a) => a.RememberEvent(c.State, c.Message, c.MachineStatePreviousName));
            Map<CreateNewStateCommand<TSagaState>>(c => new SagaStateAggregate<TSagaState>(c.State));
        }
    }

    public class SaveStateCommand<TSagaState> : Command where TSagaState : ISagaState
    {
        public TSagaState State { get; }
        public string MachineStatePreviousName { get; }
        public object Message { get; }
        public SaveStateCommand(Guid aggregateId, TSagaState state, string machineStatePreviousName, object message) : base(aggregateId)
        {
            State = state;
            MachineStatePreviousName = machineStatePreviousName;
            Message = message;
        }
    }

    public class CreateNewStateCommand<TSagaState> : Command where TSagaState : ISagaState
    {
        public TSagaState State { get; }
        public CreateNewStateCommand(Guid aggregateId, TSagaState state) : base(aggregateId)
        {
            State = state;
        }
    }

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