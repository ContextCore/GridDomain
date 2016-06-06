using System;
using GridDomain.EventSourcing;

namespace BusinessNews.Domain.BillAggregate
{
    public class BillPayedEvent:DomainEvent
    {
        public Guid BillId => SourceId;

        public BillPayedEvent(Guid billId): base(billId)
        {
            
        }
    }
}