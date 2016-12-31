using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.SkuStockAggregate
{
    public class StockAdded:DomainEvent
    {
        public int Quantity { get; }
        public string PackArticle { get;}

        public StockAdded(Guid sourceId, int quantity, string packArticle):base(sourceId)
        {
            Quantity = quantity;
            PackArticle = packArticle;
        }
    }
}