using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using Serilog;
using Shop.Domain.Aggregates.UserAggregate.Events;

namespace Shop.Domain.Aggregates.UserAggregate
{
    public class User : Aggregate
    {
        private readonly ILogger _logger = Log.Logger.ForContext<User>();

        private User(Guid id) : base(id)
        {
            Apply<UserCreated>(e =>
                               {
                                   Login = e.Login;
                                   Id = e.Id;
                                   Account = e.Account;
                               });
            Apply<SkuPurchaseOrdered>(
                                      e => { PendingOrders[e.OrderId] = new PendingOrder(e.OrderId, e.SkuId, e.Quantity, e.StockId); });
            Apply<PendingOrderCanceled>(e =>
                                        {
                                            if (!PendingOrders.Remove(e.OrderId))
                                                _logger.Warning("Could not find pending order {order} to cancel", e.OrderId);
                                        });
            Apply<PendingOrderCompleted>(e =>
                                         {
                                             if (!PendingOrders.Remove(e.OrderId))
                                                 _logger.Warning("Could not find pending order {order} to complete",
                                                                 e.OrderId);
                                         });
        }

        public User(Guid id, string login, Guid account) : this(id)
        {
            Produce(new UserCreated(id, login, account));
        }

        public string Login { get; private set; }
        public IDictionary<Guid, PendingOrder> PendingOrders { get; } = new Dictionary<Guid, PendingOrder>();
        public Guid Account { get; private set; }

        public void BuyNow(Guid skuId, int quantity, IDefaultStockProvider stockProvider)
        {
             Produce(new SkuPurchaseOrdered(Id,
                                              skuId,
                                              quantity,
                                              Guid.NewGuid(),
                                              stockProvider.GetStockForSku(skuId),
                                              Account));
        }

        public void CompleteOrder(Guid orderId)
        {
           Produce(new PendingOrderCompleted(Id, orderId));
        }

        public void CancelOrder(Guid orderId)
        {
            Produce(new PendingOrderCanceled(Id, orderId));
        }
    }
}