using System;
using System.Collections.Generic;
using NMoneys;

namespace Shop.Domain.Sagas
{
    //must not change state after method invocation 
    public interface IPriceCalculator
    { 
        Money CalculatePrice(Guid skuId, int quantity);
    }

    public class InMemoryPriceCalculator: IPriceCalculator
    {
        private IDictionary<Guid, Money> _skuPrices = new Dictionary<Guid, Money>();

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