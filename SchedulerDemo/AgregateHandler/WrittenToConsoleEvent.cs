using System;
using GridDomain.EventSourcing;

namespace SchedulerDemo.AgregateHandler
{
    public class WrittenToConsoleEvent : DomainEvent
    {
        public WrittenToConsoleEvent(Guid sourceId, DateTime? createdTime = null, Guid sagaId = new Guid()) : base(sourceId, createdTime, sagaId)
        {
        }
    }
}