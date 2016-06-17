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
            return new ScheduledCommandProcessingSaga(new ScheduledCommandProcessingSagaState(command.SagaId, ScheduledCommandProcessingSaga.States.Created));
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