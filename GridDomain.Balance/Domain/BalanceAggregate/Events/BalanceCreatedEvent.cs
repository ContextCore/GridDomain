using System;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.Domain.BalanceAggregate.Events
{
    public class BalanceCreatedEvent : DomainEvent
    {
        public BalanceCreatedEvent(Guid balanceId, Guid businessId) : base(balanceId)
        {
            BusinessId = businessId;
        }

        public Guid BalanceId => SourceId;
        public Guid BusinessId { get; }
    }
}