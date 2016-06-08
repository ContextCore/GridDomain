using System;
using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.PhoneCallDomain.ScheduledCommands
{
    public class HangUpIfNotRespondedScheduledCommand : ScheduledCommand
    {
        public Guid CallId { get; }

        public HangUpIfNotRespondedScheduledCommand(Guid callId) : base($"hangupIfNotResponded-{callId}", "calls")
        {
            CallId = callId;
        }
    }
}