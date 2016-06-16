using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingSagaFactory :
        ISagaFactory<ScheduledCommandProcessingSaga, ScheduledCommandProcessingSagaState>,
        ISagaFactory<ScheduledCommandProcessingSaga, ScheduledMessageProcessingStarted>,
        IEmptySagaFactory<ScheduledCommandProcessingSaga>

    {
        public ScheduledCommandProcessingSaga Create(ScheduledMessageProcessingStarted message)
        {
            return new ScheduledCommandProcessingSaga(new ScheduledCommandProcessingSagaState(message.SagaId, ScheduledCommandProcessingSaga.States.Created));
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