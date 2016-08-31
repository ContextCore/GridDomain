using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
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

    public class ScheduledCommandProcessingStarted : DomainEvent
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }
        public Type SuccessEventType { get; }

        public ScheduledCommandProcessingStarted(Command command, ScheduleKey key, Type successEventType) : base(key.Id, BusinessDateTime.UtcNow, key.Id)
        {
            Command = command;
            Key = key;
            SuccessEventType = successEventType;
        }

        public static ScheduledCommandProcessingStarted Create<TSuccessEvent>(Command command, ScheduleKey key) where TSuccessEvent : DomainEvent
        {
            return new ScheduledCommandProcessingStarted(command, key, typeof(TSuccessEvent));
        }

    }
}