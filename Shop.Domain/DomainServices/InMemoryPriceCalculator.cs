using System;
using System.Collections.Generic;
using NMoneys;

namespace Shop.Domain.DomainServices
{
    public class InMemoryPriceCalculator: IPriceCalculator
    {
        private readonly IDictionary<Guid, Money> _skuPrices = new Dictionary<Guid, Money>();

        public void Add(Guid skuId, Money money)
        {
            _skuPrices[skuId] = money;
        }


        public Money CalculatePrice(Guid skuId, int quantity)
        {
            Money skuPrice;
            if (!_skuPrices.TryGetValue(skuId, out skuPrice))
                throw new SkuPriceNotFoundException(skuId);

            return skuPrice*quantity;
        }
    }
}