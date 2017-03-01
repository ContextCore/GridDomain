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

        public Task<Money> CalculatePrice(Guid skuId, int quantity)
        {
            return _priceQuery.Execute(skuId).ContinueWith(t => t.Result * quantity);
        }
    }
}