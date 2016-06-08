using System;
using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.PhoneCallDomain.ScheduledCommands
{
    public class HangUpTooLongConversationSchduledCommand : ScheduledCommand
    {
        public Guid CallId { get; }

        public HangUpTooLongConversationSchduledCommand(Guid callId) : base($"hangupTooLong-{callId}", "calls")
        {
            CallId = callId;
        }
    }
}