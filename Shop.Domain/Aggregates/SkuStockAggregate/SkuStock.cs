using System;
using System.Collections.Generic;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using Shop.Domain.Aggregates.AccountAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class SkuStock : Aggregate
    {
        public TimeSpan ReservationTime { get; set; }
        public Guid SkuId { get; private set; }
        public int Quantity { get; private set; }


        private ILogger _logger = LogManager.GetLogger();

        public IDictionary<Guid, Reservation> Reservations = new Dictionary<Guid, Reservation>();

        public class Reservation
        {
            public Reservation(Guid id, int quantity, DateTime expirationDate)
            {
                Id = id;
                Quantity = quantity;
                ExpirationDate = expirationDate;
            }

            public Guid Id { get;}
            public int Quantity { get; }
            public DateTime ExpirationDate { get; }
        }

        private SkuStock(Guid id) : base(id)
        {
            Apply<SkuStockCreated>(e =>
            {
                SkuId = e.SkuId;
                Quantity = e.Quantity;
            });
            Apply<StockAdded>(e =>
            {
                //TODO: use part article
                Quantity += e.Quantity;
            });
            Apply<StockReserved>(e =>
            {
                Reservations[e.ReservationId] = new Reservation(e.ReservationId, e.Quantity, e.ExpirationDate);
                Quantity -= e.Quantity;
            });
            Apply<ReserveExpired>(e =>
            {
                Reservation reservation;
                if (!Reservations.TryGetValue(e.ReserveId, out reservation))
                {
                    _logger.Warn("Could not find expired reservation {reserve}", e.ReserveId);
                    return;
                }
                Quantity += reservation.Quantity;
                Reservations.Remove(e.ReserveId);
            });
            Apply<StockTaken>(e =>
            {
                Quantity -= e.Quantity;
            });
            Apply<StockReserveTaken>(e =>
            {
                Reservations.Remove(e.ReserveId);
            });
        }

        public SkuStock(Guid sourceId, Guid skuId, int amount, TimeSpan reservationTime):this(sourceId)
        {
            if (amount <= 0) throw new ArgumentException(nameof(amount));

            RaiseEvent(new SkuStockCreated(sourceId,skuId,amount,reservationTime));
        }

        public void Take(int quantity)
        {
            if(Quantity < quantity) throw new OutOfStockException(quantity, Quantity);
            RaiseEvent(new StockTaken(Id, quantity));
        }

        public void Take(Guid reserveId)
        {
            CancelScheduledEvents<ReserveExpired>( e => e.ReserveId == reserveId);
            RaiseEvent(new StockReserveTaken(Id,reserveId));
        }

        public void Reserve(Guid clientId, int quantity, Guid? reservationId = null)
        {
            if(Quantity < quantity)
                throw new OutOfStockException(quantity,Quantity);

            Reservation reservation;
            int quantityToReserve = quantity;
            if (Reservations.TryGetValue(clientId, out reservation))
            {
                CancelScheduledEvents<ReserveExpired>(e => e.ReserveId == reservation.Id);
                quantityToReserve += reservation.Quantity;
            }

            ReserveStock(quantityToReserve, reservationId);
        }

        private void ReserveStock(int quantity, Guid? reservationId = null)
        {
            var reserveId = reservationId ?? Guid.NewGuid();
            var expirationDate = BusinessDateTime.UtcNow + ReservationTime;

            RaiseEvent(expirationDate, new ReserveExpired(Id, reserveId));
            RaiseEvent(new StockReserved(Id, reserveId, expirationDate, quantity));
        }

        public void AddToToStock(int quantity, Guid skuId, string packArticle)
        {
            if(skuId != SkuId) throw new InvalidSkuAddException(SkuId, skuId); 
            if(quantity <= 0) throw new ArgumentException(nameof(quantity));
            RaiseEvent(new StockAdded(Id, quantity, packArticle));
        }


    }
}