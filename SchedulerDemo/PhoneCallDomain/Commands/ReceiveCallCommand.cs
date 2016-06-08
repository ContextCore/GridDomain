using System;
using GridDomain.CQRS;

namespace SchedulerDemo.PhoneCallDomain.Commands
{
    public class ReceiveCallCommand : Command
    {
        public Guid AbonentId { get; }

        public ReceiveCallCommand(Guid abonentId)
        {
            AbonentId = abonentId;
        }
    }
}