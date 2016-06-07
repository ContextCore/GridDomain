using System;
using GridDomain.EventSourcing;

namespace BusinessNews.Domain.BillAggregate
{
    public class BillPayedEvent : DomainEvent
    {
        public BillPayedEvent(Guid billId) : base(billId)
        {
        }

        public Guid BillId => SourceId;
    }
}