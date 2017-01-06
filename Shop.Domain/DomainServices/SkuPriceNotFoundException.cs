using System;

namespace Shop.Domain.DomainServices
{
    public class SkuPriceNotFoundException : Exception
    {
        public Guid SkuId { get; }

        public SkuPriceNotFoundException(Guid skuId)
        {
            SkuId = skuId;
        }
    }
}