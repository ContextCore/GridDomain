using System;
using GridDomain.CQRS;

namespace SchedulerDemo.PhoneCallDomain.Commands
{
    public class AnswerToCallCommand : Command
    {
        public Guid AbonentId { get; }

        public AnswerToCallCommand(Guid abonentId)
        {
            AbonentId = abonentId;
        }
    }
}