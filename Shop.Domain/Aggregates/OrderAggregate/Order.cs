using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing;
using NMoneys;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate;

namespace Shop.Domain.Aggregates.OrderAggregate
{
    // users can buy goods by creating orders paid from accounts
    public class Order : Aggregate
    {
        public Guid UserId { get; private set; }
        public List<OrderItem> Items { get; private set; }
        public Money TotalPrice { get; private set; }
        public int Number { get; private set; }
        public OrderStatus Status { get; private set; }

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
                Items.Add(new OrderItem(e.NumberInOrder,e.Sku,e.Quantity,e.TotalPrice));
                TotalPrice = CalculateTotalPrice();
            });

            Apply<OrderCompleted>( e => Status = e.Status);
        }

        private Money CalculateTotalPrice()
        {
            return Items.Aggregate(Money.Zero(),(m, i) =>  m + i.TotalPrice);
        }

        public Order(Guid id, int number,Guid user) : this(id)
        {
            if (number < 0) throw new NegativeOrderNumberException();
            RaiseEvent(new OrderCreated(id, number,user));
        }

        public void AddItem(Guid sku, int quantity, Money totalPrice)
        {
            if (quantity <= 0) throw new InvalidQuantityException();
            if (totalPrice < Money.Zero()) throw new InvalidMoneyException();
            if (Status != OrderStatus.Created) throw new CantAddItemsToClosedOrder();

           
            RaiseEvent(new ItemAdded(Id, sku, quantity, totalPrice, Items.Count + 1));
        }

        public void Complete()
        {
            if (Status != OrderStatus.Created) throw new CannotCompleteAlreadyClosedOrderException();
            RaiseEvent(new OrderCompleted(Id, OrderStatus.Paid));
        }
    }

    public class CannotCompleteAlreadyClosedOrderException : DomainException
    {
    }

    public class OrderCompleted : DomainEvent
    {
        public OrderStatus Status { get; }
        public OrderCompleted(Guid id, OrderStatus status):base(id)
        {
            Status = status;
        }
    }

    public class CantAddItemsToClosedOrder : DomainException
    {
    }

    public class NegativeOrderNumberException : DomainException
    {
    }

    public class InvalidMoneyException : DomainException
    {
    }

    public class InvalidQuantityException : DomainException
    {
    }
}
