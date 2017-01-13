using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using Shop.Domain.Aggregates.SkuAggregate;
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

        public async Task Handle(SkuCreated msg)
        {
            using(var context = _contextFactory())
            {
                context.Skus.Add(new Context.Sku()
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
