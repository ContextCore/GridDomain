using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using NMoneys;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.OrderAggregate.Exceptions;
using Shop.Domain.Aggregates.UserAggregate;

namespace Shop.Domain.Aggregates.OrderAggregate
{
    // users can buy goods by creating orders paid from accounts
    public class Order : Aggregate
    {
        private Order(Guid id) : base(id)
        {
            Apply<OrderCreated>(e =>
                                {
                                    UserId = e.User;
                                    Id = e.SourceId;
                                    Number = e.Number;
                                    Items = new List<OrderItem>();
                                    Status = OrderStatus.Created;
                                });

            Apply<ItemAdded>(e =>
                             {
                                 Items.Add(new OrderItem(e.NumberInOrder, e.Sku, e.Quantity, e.TotalPrice));
                                 TotalPrice = CalculateTotalPrice();
                             });

            Apply<OrderCompleted>(e => Status = e.Status);
            Apply<OrderTotalCalculated>(e => TotalPrice = e.TotalPrice);
        }

        public Order(Guid id, long number, Guid user) : this(id)
        {
            if (number < 0)
                throw new NegativeOrderNumberException();
            Produce(new OrderCreated(id, number, user));
        }

        public Guid UserId { get; private set; }
        public List<OrderItem> Items { get; private set; }
        public Money TotalPrice { get; private set; }
        public long Number { get; private set; }
        public OrderStatus Status { get; private set; }

        private Money CalculateTotalPrice()
        {
            return Items.Aggregate(Money.Zero(), (m, i) => m + i.TotalPrice);
        }

        //any discounting logic can be placed here, such as "buy 2 items for price of 1
        //calculate total call is last oin order lifetime, it means order is ready to be paid
        public void CalculateTotal()
        {
            Produce(new OrderTotalCalculated(Id, CalculateTotalPrice()));
        }

        public void AddItem(Guid sku, int quantity, Money totalPrice)
        {
            if (quantity <= 0)
                throw new InvalidQuantityException();
            if (totalPrice < Money.Zero())
                throw new InvalidMoneyException();
            if (Status != OrderStatus.Created)
                throw new CantAddItemsToClosedOrder();

            Produce(new ItemAdded(Id, sku, quantity, totalPrice, Items.Count + 1));
        }

        public void Complete()
        {
            if (Status != OrderStatus.Created)
                throw new CannotCompleteAlreadyClosedOrderException();

            Produce(new OrderCompleted(Id, OrderStatus.Paid));
        }
    }
}