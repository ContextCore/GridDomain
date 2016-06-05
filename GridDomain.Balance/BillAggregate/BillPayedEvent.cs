using System;
using GridDomain.EventSourcing;

namespace BusinessNews.Domain.Domain.BillAggregate
{
    internal class BillPayedEvent:DomainEvent
    {
        public Guid BillId => SourceId;

        public BillPayedEvent(Guid id): base(id)
        {
            
        }
    }
}