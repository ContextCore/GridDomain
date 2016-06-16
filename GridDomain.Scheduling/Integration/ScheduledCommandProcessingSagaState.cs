using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingSagaState : SagaStateAggregate<ScheduledCommandProcessingSaga.States, ScheduledCommandProcessingSaga.Transitions>
    {
        public ScheduledCommandProcessingSagaState(Guid id) : base(id)
        {

        }

        public ScheduledCommandProcessingSagaState(Guid id, ScheduledCommandProcessingSaga.States state) : base(id, state)
        {
        }
    }
}