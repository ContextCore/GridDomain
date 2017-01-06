using System;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using NMoneys;

namespace Shop.Domain.Sagas
{
    internal class BuyStockSagaData : ISagaState
    {
        public string CurrentStateName { get; set; }
        public Guid UserId { get; set; }
        public Guid SkuId { get; set; }
        public Guid AccountId { get; set; }
        public Guid OrderId { get; set; }
        public Guid StockId { get; set; }
        public int Quantity { get; set; }
        public Guid ReserveId { get; set; }
        public Money TotalOrderCost { get; set; }
    }
}