using System;
using GridDomain.CQRS;

namespace SchedulerDemo.PhoneCallDomain.Commands
{
    public class HangupCommand : Command
    {
        public Guid AbonentId { get; }

        public HangupCommand(Guid abonentId)
        {
            AbonentId = abonentId;
        }
    }
}