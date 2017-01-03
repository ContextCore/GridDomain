using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class ReserveRenewed : DomainEvent
    {
        public Guid OldReserveId { get;}
        public Guid NewReserveId { get;}

        public ReserveRenewed(Guid sourceId, Guid oldReserveId, Guid newReserveId):base(sourceId)
        {
            OldReserveId = oldReserveId;
            NewReserveId = newReserveId;
        }
    }
}