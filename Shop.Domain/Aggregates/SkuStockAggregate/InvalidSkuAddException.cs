using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class InvalidSkuAddException : DomainException
    {
        public InvalidSkuAddException(Guid expectedSku, Guid receivedSku)
        {
            ExpectedSku = expectedSku;
            ReceivedSku = receivedSku;
        }

        public Guid ExpectedSku { get; }
        public Guid ReceivedSku { get; }
    }
}