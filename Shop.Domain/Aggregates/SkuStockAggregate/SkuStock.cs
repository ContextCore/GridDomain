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

        private readonly ILogger _logger = LogManager.GetLogger();

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
                Reservations[e.ClientId] = new Reservation(e.ReservationId, e.Quantity, e.ExpirationDate);
                Quantity -= e.Quantity;
            });
            Apply<ReserveExpired>(e => CancelReservation(e.ReserveId));
            Apply<StockTaken>(e => Quantity -= e.Quantity);
            Apply<StockReserveTaken>(e => Reservations.Remove(e.ReserveId));
            Apply<ReserveRenewed>(e =>
            {
                CancelScheduledEvents<ReserveExpired>(exp => exp.ReserveId == e.OldReserveId);
                CancelReservation(e.OldReserveId);
            });

            Apply<ReserveCanceled>(e =>
            {
                 CancelScheduledEvents<ReserveExpired>(exp => exp.ReserveId == e.ReservationId);
                 CancelReservation(e.ReservationId);
            });
        }
        public SkuStock(Guid sourceId, Guid skuId, int amount, TimeSpan reservationTime) : this(sourceId)
        {
            if (amount <= 0) throw new ArgumentException(nameof(amount));
            RaiseEvent(new SkuStockCreated(sourceId, skuId, amount, reservationTime));
        }

        private void CancelReservation(Guid reserveId)
        {
            Reservation reservation;
            if (!Reservations.TryGetValue(reserveId, out reservation))
            {
                _logger.Warn("Could not find expired reservation {reserve}", reserveId);
                return;
            }
            Quantity += reservation.Quantity;
            Reservations.Remove(reserveId);
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

        public void Reserve(Guid clientId, int quantity, Guid? reservationId = null, DateTime? reservationStartTime = null)
        {
            if(Quantity < quantity)
                throw new OutOfStockException(quantity,Quantity);

            Reservation oldReservation;
            int quantityToReserve = quantity;
            var newReserveId = reservationId ?? Guid.NewGuid();
            var expirationDate = (reservationStartTime ?? BusinessDateTime.UtcNow) + ReservationTime;

            if (Reservations.TryGetValue(clientId, out oldReservation))
            {
                quantityToReserve += oldReservation.Quantity;
                RaiseEvent(new ReserveRenewed(Id, clientId, oldReservation.Id, newReserveId));
            }

            RaiseEvent(new StockReserved(Id, clientId, newReserveId, expirationDate, quantityToReserve));
            RaiseEvent(expirationDate, new ReserveExpired(Id, newReserveId),newReserveId);
        }

        public void AddToToStock(int quantity, Guid skuId, string packArticle)
        {
            if(skuId != SkuId) throw new InvalidSkuAddException(SkuId, skuId); 
            if(quantity <= 0) throw new ArgumentException(nameof(quantity));
            RaiseEvent(new StockAdded(Id, quantity, packArticle));
        }

        public void Cancel(Guid reserveId)
        {
            RaiseEvent(new ReserveCanceled(Id, reserveId));
        }
    }
}