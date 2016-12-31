using System;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class InvalidSkuAddException : Exception
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