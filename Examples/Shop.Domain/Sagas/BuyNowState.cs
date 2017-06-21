using System;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace Shop.Domain.Sagas
{
    public class BuyNowState : ISagaState
    {
        public BuyNowState(Guid id, string currentStateName)
        {
            CurrentStateName = currentStateName;
            Id = id;
        }

        public Guid UserId { get; set; }
        public Guid SkuId { get; set; }
        public Guid AccountId { get; set; }
        public Guid OrderId { get; set; }
        public Guid StockId { get; set; }
        public int Quantity { get; set; }
        public Guid ReserveId { get; set; }
        public int OrderWarReservedStatus { get; set; }

        public Guid Id { get; }
        public string CurrentStateName { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}