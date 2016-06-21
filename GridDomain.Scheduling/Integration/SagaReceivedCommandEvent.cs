using System;
using GridDomain.CQRS;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class SagaReceivedCommandEvent
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }
        public Type SuccessEventType { get; }

        public SagaReceivedCommandEvent(Command command, ScheduleKey key, Type successEventType)
        {
            Command = command;
            Key = key;
            SuccessEventType = successEventType;
        }
    }
}