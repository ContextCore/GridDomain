using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using Shop.Domain.Aggregates.SkuAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.ReadModel
{
    public class SkuProjectionBuilder : IHandler<SkuCreated>
    {
        private readonly Func<ShopDbContext> _contextFactory;

        public SkuProjectionBuilder(Func<ShopDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Handle(SkuCreated msg, IMessageMetadata metadata = null)
        {
            using (var context = _contextFactory())
            {
                context.Skus.Add(new Sku
                                 {
                                     Id = msg.SourceId,
                                     Article = msg.Article,
                                     Created = msg.CreatedTime,
                                     LastModified = msg.CreatedTime,
                                     Name = msg.Name,
                                     Number = msg.Number,
                                     Price = msg.Price.Amount,
                                     Currency = msg.Price.CurrencyCode.ToString()
                                 });
                await context.SaveChangesAsync();
            }
        }
    }
}