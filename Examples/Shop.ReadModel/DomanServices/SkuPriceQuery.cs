using System;
using System.Threading.Tasks;
using NMoneys;
using Shop.Domain.DomainServices.PriceCalculator;
using Shop.ReadModel.Context;

namespace Shop.ReadModel.DomanServices
{
    public class SkuPriceQuery : ISkuPriceQuery
    {
        private readonly Func<ShopDbContext> _contextFactory;

        public SkuPriceQuery(Func<ShopDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Money> Execute(Guid skuId)
        {
            using (var context = _contextFactory())
            {
                var sku = await context.Skus.FindAsync(skuId);
                return new Money(sku.Price, Currency.Get(sku.Currency));
            }
        }
    }
}