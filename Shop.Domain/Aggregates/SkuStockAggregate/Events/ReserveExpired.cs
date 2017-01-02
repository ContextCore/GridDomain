using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class ReserveExpired : DomainEvent
    {
        public Guid ReserveId { get;}

        public ReserveExpired(Guid sourceId, Guid reserveId):base(sourceId)
        {
            ReserveId = reserveId;
        }
    }
}