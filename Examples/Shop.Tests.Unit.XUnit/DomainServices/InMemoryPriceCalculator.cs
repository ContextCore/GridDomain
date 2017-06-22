using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NMoneys;
using Shop.Domain.DomainServices.PriceCalculator;

namespace Shop.Tests.Unit.XUnit.DomainServices
{
    public class InMemoryPriceCalculator : IPriceCalculator
    {
        private readonly IDictionary<Guid, Money> _skuPrices = new Dictionary<Guid, Money>();

        public Task<Money> CalculatePrice(Guid skuId, int quantity)
        {
            Money skuPrice;
            if (!_skuPrices.TryGetValue(skuId, out skuPrice))
                throw new SkuPriceNotFoundException(skuId);

            return Task.FromResult(skuPrice * quantity);
        }

        public void Add(Guid skuId, Money money)
        {
            _skuPrices[skuId] = money;
        }
    }
}