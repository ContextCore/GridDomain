using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingSagaFactory :
        ISagaFactory<ScheduledCommandProcessingSaga, ScheduledCommandProcessingSagaState>,
        ISagaFactory<ScheduledCommandProcessingSaga, ScheduledCommandProcessingStarted>,
        IEmptySagaFactory<ScheduledCommandProcessingSaga>

    {
        public ScheduledCommandProcessingSaga Create(ScheduledCommandProcessingStarted command)
        {
            var sagaState = new ScheduledCommandProcessingSagaState(command.SagaId, ScheduledCommandProcessingSaga.States.Created, command.Command, command.Key, command.SuccessEventType);
            return new ScheduledCommandProcessingSaga(sagaState);
        }

        public ScheduledCommandProcessingSaga Create(ScheduledCommandProcessingSagaState state)
        {
            return new ScheduledCommandProcessingSaga(state);
        }

        public ScheduledCommandProcessingSaga Create()
        {
            return new ScheduledCommandProcessingSaga(new ScheduledCommandProcessingSagaState(Guid.Empty));
        }
    }
}