using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingSagaState : SagaStateAggregate<ScheduledCommandProcessingSaga.States, ScheduledCommandProcessingSaga.Transitions>
    {
        private ScheduledCommandProcessingSagaState(Guid id) : base(id)
        {
        }

        public ScheduledCommandProcessingSagaState(Guid id, ScheduledCommandProcessingSaga.States machineState, Command command, ScheduleKey key, Type successEventType) : base(id, machineState)
        {
            RaiseEvent(new SagaReceivedCommandEvent(command, key, successEventType));
        }

        public Command Command { get; private set; }
        public ScheduleKey Key { get; private set; }
        public Type SuccessEventType { get; private set; }

        public void Apply(SagaReceivedCommandEvent @event)
        {
            Command = @event.Command;
            Key = @event.Key;
            SuccessEventType = @event.SuccessEventType;
        }
    }
}