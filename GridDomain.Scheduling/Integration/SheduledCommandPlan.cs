using System;
using GridDomain.CQRS;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class SheduledCommandPlan
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }
        public Type SuccessEventType { get; }

        private SheduledCommandPlan(Command command, ScheduleKey key, Type successEventType) 
            //base(key.Id, DateTimeFacade.UtcNow, key.Id)
        {
            Command = command;
            Key = key;
            SuccessEventType = successEventType;
        }
    }
}