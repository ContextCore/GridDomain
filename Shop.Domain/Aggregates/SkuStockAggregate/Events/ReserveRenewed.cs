using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class ReserveRenewed : DomainEvent
    {
        public Guid ClientId { get; }
        public Guid OldReserveId { get;}
        public Guid NewReserveId { get;}

        public ReserveRenewed(Guid sourceId, Guid clientId, Guid oldReserveId, Guid newReserveId):base(sourceId)
        {
            ClientId = clientId;
            OldReserveId = oldReserveId;
            NewReserveId = newReserveId;
        }
    }
}