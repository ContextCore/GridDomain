using System;

namespace Shop.Domain.Aggregates.UserAggregate
{
    public interface IDefaultStockProvider
    {
        Guid GetStockForSku(Guid skuId);
    }
}