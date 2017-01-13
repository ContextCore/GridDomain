using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.ReadModel.Context;
using Order = Shop.ReadModel.Context.Order;
using OrderItem = Shop.ReadModel.Context.OrderItem;

namespace Shop.ReadModel
{
    public class OrdersProjectionBuilder : IHandler<OrderCreated>,
                                           IHandler<ItemAdded>,            
                                           IHandler<OrderCompleted>

    {
        private readonly Func<ShopDbContext> _contextFactory;

        public OrdersProjectionBuilder(Func<ShopDbContext> contextFactory )
        {
            _contextFactory = contextFactory;
        }

        public async Task Handle(OrderCreated msg)
        {
            using (var context = _contextFactory())
            {
                var user = await context.Users.FindAsync(msg.User);
                var userLogin = user.Login;

                context.Orders.Add(new Order()
                {
                    Id = msg.SourceId,
                    Number = msg.Number,
                    Status = msg.Status,
                    UserId = msg.User,
                    UserLogin = userLogin,
                    Created = msg.CreatedTime,
                    LastModified = msg.CreatedTime
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(ItemAdded msg)
        {
            using (var context = _contextFactory())
            {
                var sku = await context.Goods.FindAsync(msg.Sku);
                context.OrderItems.Add(new OrderItem()
                {
                    OrderId = msg.SourceId,
                    Created = msg.CreatedTime,
                    Currency = msg.TotalPrice.CurrencyCode.ToString(),
                    NumberInOrder = msg.NumberInOrder,
                    Quantity = msg.Quantity,
                    SkuId = msg.Sku,
                    SkuName = sku.Name,
                    TotalPrice = msg.TotalPrice.Amount
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(OrderCompleted msg)
        {
            using (var context = _contextFactory())
            {
                var order = await context.Orders.FindAsync(msg.SourceId);
                order.Status = msg.Status;
                order.LastModified = msg.CreatedTime;
                await context.SaveChangesAsync();
            }
        }
    }
}
