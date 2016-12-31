using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class SkuStock : Aggregate
    {
        public Guid SkuId { get; private set; }
        public int Quantity { get; private set; }

        private IDictionary<Guid, Reservation> _reservations = new Dictionary<Guid, Reservation>();

        class Reservation
        {
            public Reservation(int quantity, DateTime expirationDate)
            {
                Quantity = quantity;
                ExpirationDate = expirationDate;
            }

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
        }

        public SkuStock(Guid sourceId, Guid skuId, int amount):this(sourceId)
        {
            if (amount <= 0) throw new ArgumentException(nameof(amount));

            RaiseEvent(new SkuStockCreated(sourceId,skuId,amount));
        }

        public void AddToToStock(int quantity, Guid skuId, string packArticle)
        {
            if(skuId != SkuId) throw new InvalidSkuAddException(SkuId, skuId); 
            if(quantity <= 0) throw new ArgumentException(nameof(quantity));
            RaiseEvent(new StockAdded(Id, quantity, packArticle));
        }


    }
}