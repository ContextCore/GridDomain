using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingStarted : DomainEvent
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }

        public ScheduledCommandProcessingStarted(Command command, ScheduleKey key) : base(key.Id, DateTime.UtcNow, key.Id)
        {
            Command = command;
            Key = key;
        }
    }
}