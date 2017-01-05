using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using Shop.Domain.Aggregates.UserAggregate.Events;

namespace Shop.Domain.Aggregates.UserAggregate
{
    public interface IDefaultStockProvider
    {
        Guid GetStockForSku(Guid skuId);
    }

    public class User : Aggregate
    {
        private ILogger _logger = LogManager.GetLogger();
        public string Login { get; private set; }
        public IDictionary<Guid,PendingOrder> PendingOrders { get; } = new Dictionary<Guid, PendingOrder>();
        public Guid Account { get; private set; }
        private User(Guid id) : base(id)
        {
            Apply<UserCreated>(e =>
            {
                Login = e.Login;
                Id = e.Id;
                Account = e.Account;
            });
            Apply<SkuPurchaseOrdered>(e =>
            {
                PendingOrders[e.OrderId] = new PendingOrder(e.OrderId, e.SkuId, e.Quantity,e.StockId);
            });
            Apply<PendingOrderCanceled>(e =>
            {
                if (!PendingOrders.Remove(e.OrderId))
                    _logger.Warn("Could not find pending order {order} to cancel",e.OrderId);
            });
            Apply<PendingOrderCompleted>(e =>
            {
                if (!PendingOrders.Remove(e.OrderId))
                    _logger.Warn("Could not find pending order {order} to complete", e.OrderId);
            });
        }

        public User(Guid id, string login, Guid account) : this(id)
        {
            RaiseEvent(new UserCreated(id, login, account));
        }

        public void BuyNow(Guid skuId, int quantity, IDefaultStockProvider stockProvider)
        {
            RaiseEvent(new SkuPurchaseOrdered(Id,skuId,quantity,Guid.NewGuid(),stockProvider.GetStockForSku(skuId)));
        }

        public void CompleteOrder(Guid orderId)
        {
            RaiseEvent(new PendingOrderCompleted(Id, orderId));
        }

        public void CancelOrder(Guid orderId)
        {
            RaiseEvent(new PendingOrderCanceled(Id, orderId));
        }
    }
}