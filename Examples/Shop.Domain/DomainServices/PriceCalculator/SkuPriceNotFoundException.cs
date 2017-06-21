using System;

namespace Shop.Domain.DomainServices.PriceCalculator
{
    public class SkuPriceNotFoundException : Exception
    {
        public SkuPriceNotFoundException(Guid skuId)
        {
            SkuId = skuId;
        }

        public Guid SkuId { get; }
    }
}