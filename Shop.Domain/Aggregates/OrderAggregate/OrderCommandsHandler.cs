using GridDomain.CQRS.Messaging.MessageRouting;
using Shop.Domain.Aggregates.OrderAggregate.Commands;

namespace Shop.Domain.Aggregates.OrderAggregate
{
    public class OrderCommandsHandler : AggregateCommandsHandler<Order>
    {
        public OrderCommandsHandler()
        {
            Map<CreateOrderCommand>(c => c.OrderId,
                                   c => new Order(c.OrderId, c.OrderNumber, c.UserId));

            Map<AddItemToOrderCommand>(c => c.OrderId,
                                      (c,a) => a.AddItem(c.SkuId, c.Quantity, c.TotalPrice));

            Map<CompleteOrderCommand>(c => c.OrderId,
                                      (c,a) => a.Complete());
        }
    }
}