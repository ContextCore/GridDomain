using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate.Events
{
    public class StockAdded : DomainEvent
    {
        public StockAdded(Guid sourceId, int quantity, string packArticle) : base(sourceId)
        {
            Quantity = quantity;
            PackArticle = packArticle;
        }

        public int Quantity { get; }
        public string PackArticle { get; }
    }
}