using System;
using System.Collections.Generic;
using GridDomain.Common;
using GridDomain.EventSourcing;
using Serilog;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class SkuStock : Aggregate
    {
        private readonly ILogger _logger = Log.Logger.ForContext<SkuStock>();

        public readonly IDictionary<Guid, Reservation> Reservations = new Dictionary<Guid, Reservation>();

        private SkuStock(Guid id) : base(id)
        {
            Apply<SkuStockCreated>(e =>
                                   {
                                       SkuId = e.SkuId;
                                       Quantity = e.Quantity;
                                       ReservationTime = e.ReservationTime;
                                       Id = e.SourceId;
                                   });
            //TODO: use part article

            Apply<StockAdded>(e => Quantity += e.Quantity);
            Apply<StockReserved>(e =>
                                 {
                                     Reservations[e.ReserveId] = new Reservation(e.Quantity, e.ExpirationDate);
                                     Quantity -= e.Quantity;
                                 });
            Apply<ReserveExpired>(e => CancelReservation(e.ReserveId));
            Apply<StockTaken>(e => Quantity -= e.Quantity);
            Apply<StockReserveTaken>(e => Reservations.Remove(e.ReserveId));
            Apply<ReserveRenewed>(e =>
                                  {
                                      CancelScheduledEvents<ReserveExpired>(exp => exp.ReserveId == e.ReserveId);
                                      CancelReservation(e.ReserveId);
                                  });

            Apply<ReserveCanceled>(e =>
                                   {
                                       CancelScheduledEvents<ReserveExpired>(exp => exp.ReserveId == e.ReserveId);
                                       CancelReservation(e.ReserveId);
                                   });
        }

        public SkuStock(Guid sourceId, Guid skuId, int amount, TimeSpan reservationTime) : this(sourceId)
        {
            if (amount <= 0) throw new ArgumentException(nameof(amount));
            RaiseEvent(new SkuStockCreated(sourceId, skuId, amount, reservationTime));
        }

        public TimeSpan ReservationTime { get; set; }
        public Guid SkuId { get; private set; }
        public int Quantity { get; private set; }

        private void CancelReservation(Guid reserveId)
        {
            Reservation reservation;
            if (!Reservations.TryGetValue(reserveId, out reservation))
            {
                _logger.Warning("Could not find expired reservation {reserve}", reserveId);
                return;
            }
            Quantity += reservation.Quantity;
            Reservations.Remove(reserveId);
        }

        public void Take(int quantity)
        {
            if (Quantity < quantity) throw new OutOfStockException(quantity, Quantity);
            RaiseEvent(new StockTaken(Id, quantity));
        }

        public void Take(Guid reserveId)
        {
            CancelScheduledEvents<ReserveExpired>(e => e.ReserveId == reserveId);
            RaiseEvent(new StockReserveTaken(Id, reserveId));
        }

        public void Reserve(Guid reserveId, int quantity, DateTime? reservationStartTime = null)
        {
            if (Quantity < quantity) throw new OutOfStockException(quantity, Quantity);

            Reservation oldReservation;
            var quantityToReserve = quantity;
            var expirationDate = (reservationStartTime ?? BusinessDateTime.UtcNow) + ReservationTime;

            if (Reservations.TryGetValue(reserveId, out oldReservation))
            {
                quantityToReserve += oldReservation.Quantity;
                RaiseEvent(new ReserveRenewed(Id, reserveId));
            }

            RaiseEvent(new StockReserved(Id, reserveId, expirationDate, quantityToReserve));
            RaiseEvent(new ReserveExpired(Id, reserveId), expirationDate);
        }

        public void AddToToStock(int quantity, Guid skuId, string packArticle)
        {
            if (skuId != SkuId) throw new InvalidSkuAddException(SkuId, skuId);
            if (quantity <= 0) throw new ArgumentException(nameof(quantity));
            RaiseEvent(new StockAdded(Id, quantity, packArticle));
        }

        public void Cancel(Guid reserveId)
        {
            RaiseEvent(new ReserveCanceled(Id, reserveId));
        }
    }
}