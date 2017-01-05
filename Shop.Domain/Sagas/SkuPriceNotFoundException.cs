using System;

namespace Shop.Domain.Sagas
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