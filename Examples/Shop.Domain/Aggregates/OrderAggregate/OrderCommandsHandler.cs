using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Infrastructure;

namespace Shop.Domain.Aggregates.OrderAggregate
{
    public class OrderCommandsHandler : AggregateCommandsHandler<Order>
    {
        private const string OrdersSequenceName = "OrdersSequence";

        public OrderCommandsHandler(ISequenceProvider sequenceProvider)
        {
            Map<CreateOrderCommand>(c => new Order(c.OrderId, sequenceProvider.GetNext(OrdersSequenceName), c.UserId));

            Map<AddItemToOrderCommand>((c, a) => a.AddItem(c.SkuId, c.Quantity, c.TotalPrice));

            Map<CalculateOrderTotalCommand>((c, a) => a.CalculateTotal());

            Map<CompleteOrderCommand>((c, a) => a.Complete());
        }
    }
}