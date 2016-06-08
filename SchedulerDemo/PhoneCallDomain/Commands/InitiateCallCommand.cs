using System;
using GridDomain.CQRS;

namespace SchedulerDemo.PhoneCallDomain.Commands
{
    public class InitiateCallCommand : Command
    {
        public Guid InitiatorId { get; }
        public Guid AbonentId { get; }

        public InitiateCallCommand(Guid initiatorId, Guid abonentId)
        {
            InitiatorId = initiatorId;
            AbonentId = abonentId;
        }
    }
}