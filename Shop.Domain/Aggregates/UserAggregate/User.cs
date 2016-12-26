using System;
using GridDomain.EventSourcing;
using Shop.Domain.Aggregates.UserAggregate.Events;

namespace Shop.Domain.Aggregates.UserAggregate
{
   // public class OrderHistory
   // {
   //     public OrderHistory(Guid order, OrderStatus status)
   //     {
   //         Order = order;
   //         Status = status;
   //     }
   //
   //     public Guid Order { get; }
   //     public OrderStatus Status { get; }
   // }

    public enum OrderStatus
    {
        Created,
        Paid
    }

    public class User : Aggregate
    {
        public string Login { get; private set; }
      //  public List<OrderHistory> History { get; private set; }
        public Guid Account { get; private set; }

        private User(Guid id) : base(id)
        {
            Apply<UserCreated>(e =>
            {
                Login = e.Login;
                Id = e.Id;
                Account = e.Account;
                //History = new List<OrderHistory>();
            });
        }

        public User(Guid id, string login, Guid account) : this(id)
        {
            RaiseEvent(new UserCreated(id, login, account));
        }

        //public void CreateNewOrder(Guid orderId)
        //{
        //    RaiseEvent(new OrderAdded);
        //}
    }
}