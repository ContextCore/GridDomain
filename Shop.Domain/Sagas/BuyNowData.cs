using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using NMoneys;

namespace Shop.Domain.Sagas
{
    public class BuyNowData : ISagaState
    {
        public BuyNowData(string stateName)
        {
            CurrentStateName = stateName;
        }

        public string CurrentStateName { get; set; }
        public Guid UserId { get; set; }
        public Guid SkuId { get; set; }
        public Guid AccountId { get; set; }
        public Guid OrderId { get; set; }
        public Guid StockId { get; set; }
        public int Quantity { get; set; }
        public Guid ReserveId { get; set; }
        public int OrderWarReserved_Status { get; set; }
    }
}