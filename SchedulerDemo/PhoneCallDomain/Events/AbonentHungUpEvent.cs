using System;
using GridDomain.EventSourcing;

namespace SchedulerDemo.PhoneCallDomain.Events
{
    public class AbonentHungUpEvent : DomainEvent
    {
        public Guid AbonentId => SourceId;

        public AbonentHungUpEvent(Guid abonentId) : base(abonentId)
        {
        }
    }
}