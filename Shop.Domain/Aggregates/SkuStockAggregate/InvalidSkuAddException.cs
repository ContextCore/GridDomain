using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class InvalidSkuAddException : DomainException
    {
        public Guid ExpectedSku { get; }
        public Guid ReceivedSku { get;}

        public InvalidSkuAddException(Guid expectedSku, Guid receivedSku)
        {
            this.ExpectedSku = expectedSku;
            this.ReceivedSku = receivedSku;
        }
    }
}