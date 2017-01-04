using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;
using SkuStock = Shop.ReadModel.Context.SkuStock;

namespace Shop.ReadModel
{
    public class SkuStockProjectionBuilder : IEventHandler<SkuStockCreated>,
                                             IEventHandler<StockAdded>,
                                             IEventHandler<StockReserved>,
                                             IEventHandler<ReserveExpired>,
                                             IEventHandler<StockTaken>,
                                             IEventHandler<StockReserveTaken>,
                                             IEventHandler<ReserveRenewed>,
                                             IEventHandler<ReserveCanceled>
    {
        private readonly Func<ShopDbContext> _contextFactory;

        public SkuStockProjectionBuilder(Func<ShopDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Handle(SkuStockCreated msg)
        {
            using (var context = _contextFactory())
            {
                var skuStock = new SkuStock
                {
                    Id = msg.SourceId,
                    AvailableQuantity = msg.Quantity,
                    Created = msg.CreatedTime,
                    CustomersReservationsTotal = 0,
                    LastModified = msg.CreatedTime,
                    ReservedQuantity = 0,
                    SkuId = msg.SkuId,
                    TotalQuantity = msg.Quantity
                };

                context.SkuStocks.Add(skuStock);
                context.SaveChanges();
            }
        }

        public void Handle(StockAdded msg)
        {
            using (var context = _contextFactory())
            {
                var skuStock = context.SkuStocks.Find(msg.SourceId);
                if (skuStock == null)
                    throw new SkuStockEntryNotFoundException(msg.SourceId);

                var history = new SkuStockHistory()
                {
                    OldAvailableQuantity = skuStock.AvailableQuantity,
                    OldTotalQuantity = skuStock.TotalQuantity,
                    OldReservedQuantity = skuStock.ReservedQuantity,
                    Operation = SkuStockOperation.Added,
                    Quanity = msg.Quantity,
                    StockId = msg.SourceId
                };

                skuStock.AvailableQuantity += msg.Quantity;
                skuStock.TotalQuantity += msg.Quantity;
                skuStock.LastModified = msg.CreatedTime;

                history.NewAvailableQuantity = skuStock.AvailableQuantity;
                history.NewTotalQuantity = skuStock.TotalQuantity;
                history.NewReservedQuantity = skuStock.ReservedQuantity;

                context.StockHistory.Add(history);
                context.SaveChanges();
            }
        }


        public void Handle(ReserveCanceled msg)
        {
            throw new NotImplementedException();
        }

        public void Handle(ReserveExpired msg)
        {
            throw new NotImplementedException();
        }

        public void Handle(StockReserveTaken msg)
        {
            throw new NotImplementedException();
        }

        public void Handle(ReserveRenewed msg)
        {
            throw new NotImplementedException();
        }

        public void Handle(StockTaken msg)
        {
            throw new NotImplementedException();
        }

    
     

        public void Handle(StockReserved msg)
        {
            throw new NotImplementedException();
        }
    }
}
