using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingSagaState : SagaStateAggregate<ScheduledCommandProcessingSaga.States, ScheduledCommandProcessingSaga.Transitions>
    {
        public ScheduledCommandProcessingSagaState(Guid id) : base(id)
        {
        }

        public ScheduledCommandProcessingSagaState(Guid id, ScheduledCommandProcessingSaga.States state, Command command, ScheduleKey key) : base(id, state)
        {
            var sagaReceivedCommandEvent = new SagaReceivedCommandEvent(command, key);
            Apply(sagaReceivedCommandEvent);
            RaiseEvent(sagaReceivedCommandEvent);
        }

        public Command Command { get; private set; }
        public ScheduleKey Key { get; private set; }

        public void Apply(SagaReceivedCommandEvent @event)
        {
            Command = @event.Command;
            Key = @event.Key;
        }
    }
}