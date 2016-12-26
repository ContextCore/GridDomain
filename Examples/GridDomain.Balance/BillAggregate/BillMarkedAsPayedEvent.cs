using System;
using GridDomain.EventSourcing;

namespace BusinessNews.Domain.BillAggregate
{
    public class BillMarkedAsPayedEvent : DomainEvent
    {
        public BillMarkedAsPayedEvent(Guid billId) : base(billId)
        {
        }

        public Guid BillId => SourceId;
    }
}