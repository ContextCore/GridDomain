using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.Infrastructure;
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
        private readonly ISequenceProvider _sequenceProvider;
        private const string SkuHistorySequenceName = "SkuHistory";

        public SkuStockProjectionBuilder(Func<ShopDbContext> contextFactory, ISequenceProvider sequenceProvider)
        {
            _sequenceProvider = sequenceProvider;
            _contextFactory = contextFactory;
        }

        public void Handle(SkuStockCreated msg)
        {
            using (var context = _contextFactory())
            {
                var skuStock = new SkuStock
                {
                    Id = msg.SourceId,
                    AvailableQuantity = 0,
                    Created = msg.CreatedTime,
                    CustomersReservationsTotal = 0,
                    LastModified = msg.CreatedTime,
                    ReservedQuantity = 0,
                    SkuId = msg.SkuId,
                    TotalQuantity = 0
                };

                var history = CreateHistory(skuStock, StockOperation.Created, msg.Quantity);

                skuStock.TotalQuantity = msg.Quantity;
                skuStock.AvailableQuantity = msg.Quantity;

                FillNewQuantities(history, skuStock);

                context.SkuStocks.Add(skuStock);
                context.StockHistory.Add(history);
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

                var history = CreateHistory(skuStock, StockOperation.Added, msg.Quantity);

                skuStock.AvailableQuantity += msg.Quantity;
                skuStock.TotalQuantity += msg.Quantity;
                skuStock.LastModified = msg.CreatedTime;

                FillNewQuantities(history, skuStock);

                context.StockHistory.Add(history);
                context.SaveChanges();
            }
        }

        public void Handle(StockReserved msg)
        {
            using (var context = _contextFactory())
            {
                var skuStock = context.SkuStocks.Find(msg.SourceId);
                if (skuStock == null)
                    throw new SkuStockEntryNotFoundException(msg.SourceId);

                var reserve = new SkuReserve()
                {
                    Created = msg.CreatedTime,
                    CustomerId = msg.ClientId,
                    ExpirationDate = msg.ExpirationDate,
                    Quantity = msg.Quantity,
                    SkuId = skuStock.SkuId,
                    StockId = msg.SourceId
                };

                var history = CreateHistory(skuStock, StockOperation.Reserved, msg.Quantity);

                skuStock.AvailableQuantity -= msg.Quantity;
                skuStock.ReservedQuantity += msg.Quantity;
                skuStock.CustomersReservationsTotal ++;
                skuStock.LastModified = msg.CreatedTime;

                FillNewQuantities(history, skuStock);

                context.StockHistory.Add(history);
                context.StockReserves.Add(reserve);

                context.SaveChanges();
            }
        }

        private SkuStockHistory CreateHistory(SkuStock skuStock, StockOperation stockOperation, int quantity)
        {
            return new SkuStockHistory()
            {
                OldAvailableQuantity = skuStock.AvailableQuantity,
                OldTotalQuantity = skuStock.TotalQuantity,
                OldReservedQuantity = skuStock.ReservedQuantity,
                Operation = stockOperation,
                Quanity = quantity,
                StockId = skuStock.Id,
                Number = _sequenceProvider.GetNext(SkuHistorySequenceName)
            };
        }

        private void FillNewQuantities(SkuStockHistory history, SkuStock skuStock)
        {
            history.NewAvailableQuantity = skuStock.AvailableQuantity;
            history.NewTotalQuantity = skuStock.TotalQuantity;
            history.NewReservedQuantity = skuStock.ReservedQuantity;
        }
        public void Handle(ReserveCanceled msg)
        {
            using (var context = _contextFactory())
            {
                var skuStock = context.SkuStocks.Find(msg.SourceId);
                if (skuStock == null)
                    throw new SkuStockEntryNotFoundException(msg.SourceId);

                var reserve = context.StockReserves.Find(msg.SourceId, msg.CustomerId);
                if (reserve == null)
                    throw new ReserveEntryNotFoundException(msg.SourceId);

                var history = CreateHistory(skuStock, StockOperation.ReserveCanceled, reserve.Quantity);
                
                skuStock.AvailableQuantity += reserve.Quantity;
                skuStock.ReservedQuantity -= reserve.Quantity;
                skuStock.LastModified = msg.CreatedTime;
                skuStock.CustomersReservationsTotal --;

                FillNewQuantities(history, skuStock);

                context.StockReserves.Remove(reserve);
                context.StockHistory.Add(history);

                context.SaveChanges();
            }
        }

        public void Handle(ReserveExpired msg)
        {
            using (var context = _contextFactory())
            {
                var skuStock = context.SkuStocks.Find(msg.SourceId);
                if (skuStock == null)
                    throw new SkuStockEntryNotFoundException(msg.SourceId);

                var reserve = context.StockReserves.Find(msg.SourceId, msg.CustomerId);
                if (reserve == null)
                    throw new ReserveEntryNotFoundException(msg.SourceId);

                var history = CreateHistory(skuStock, StockOperation.ReserveExpired, reserve.Quantity);

                skuStock.AvailableQuantity += reserve.Quantity;
                skuStock.ReservedQuantity -= reserve.Quantity;
                skuStock.LastModified = msg.CreatedTime;
                skuStock.CustomersReservationsTotal--;

                FillNewQuantities(history, skuStock);

                context.StockReserves.Remove(reserve);
                context.StockHistory.Add(history);

                context.SaveChanges();
            }
        }

        public void Handle(StockReserveTaken msg)
        {
            using (var context = _contextFactory())
            {
                var skuStock = context.SkuStocks.Find(msg.SourceId);
                if (skuStock == null)
                    throw new SkuStockEntryNotFoundException(msg.SourceId);

                var reserve = context.StockReserves.Find(msg.SourceId, msg.CustomerId);
                if (reserve == null)
                    throw new ReserveEntryNotFoundException(msg.SourceId);

                var history = CreateHistory(skuStock, StockOperation.ReserveTaken, reserve.Quantity);

                skuStock.ReservedQuantity -= reserve.Quantity;
                skuStock.TotalQuantity -= reserve.Quantity;
                skuStock.LastModified = msg.CreatedTime;
                skuStock.CustomersReservationsTotal--;

                FillNewQuantities(history, skuStock);

                context.StockReserves.Remove(reserve);
                context.StockHistory.Add(history);

                context.SaveChanges();
            }
        }

        public void Handle(ReserveRenewed msg)
        {
            using (var context = _contextFactory())
            {
                var skuStock = context.SkuStocks.Find(msg.SourceId);
                if (skuStock == null)
                    throw new SkuStockEntryNotFoundException(msg.SourceId);

                var reserve = context.StockReserves.Find(msg.SourceId, msg.CustomerId);
                if (reserve == null)
                    throw new ReserveEntryNotFoundException(msg.SourceId);

                var history = CreateHistory(skuStock, StockOperation.ReserveRenewed, reserve.Quantity);

                skuStock.ReservedQuantity -= reserve.Quantity;
                skuStock.AvailableQuantity += reserve.Quantity;
                skuStock.LastModified = msg.CreatedTime;
                skuStock.CustomersReservationsTotal--;

                FillNewQuantities(history, skuStock);

                context.StockReserves.Remove(reserve);
                context.StockHistory.Add(history);

                context.SaveChanges();
            }
        }

        public void Handle(StockTaken msg)
        {
            using (var context = _contextFactory())
            {
                var skuStock = context.SkuStocks.Find(msg.SourceId);
                if (skuStock == null)
                    throw new SkuStockEntryNotFoundException(msg.SourceId);

                var history = CreateHistory(skuStock, StockOperation.Taken, msg.Quantity);

                skuStock.AvailableQuantity -= msg.Quantity;
                skuStock.TotalQuantity -= msg.Quantity;
                skuStock.LastModified = msg.CreatedTime;

                FillNewQuantities(history, skuStock);

                context.StockHistory.Add(history);
                context.SaveChanges();
            }
        }
    }
}
