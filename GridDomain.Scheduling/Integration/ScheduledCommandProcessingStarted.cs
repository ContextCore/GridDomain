using System;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingStarted : DomainEvent
    {
        public ScheduledCommand Command { get; set; }

        public ScheduledCommandProcessingStarted(Guid sourceId, ScheduledCommand command) : base(sourceId, DateTime.UtcNow)
        {
            Command = command;
        }
    }
}