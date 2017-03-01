using System;
using System.Threading.Tasks;
using NMoneys;

namespace Shop.Domain.DomainServices.PriceCalculator
{
    public class SqlPriceCalculator : IPriceCalculator
    {
        private readonly ISkuPriceQuery _priceQuery;

        public SqlPriceCalculator(ISkuPriceQuery priceQuery)
        {
            _priceQuery = priceQuery;
        }

        public async Task<Money> CalculatePrice(Guid skuId, int quantity)
        {
            return await _priceQuery.Execute(skuId) * quantity;
        }
    }
}