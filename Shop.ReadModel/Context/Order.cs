using System;
using System.ComponentModel.DataAnnotations;
using Shop.Domain.Aggregates.UserAggregate;

namespace Shop.ReadModel.Context
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public long Number { get; set; }
        public string UserLogin { get; set; }
        public Guid UserId { get; set; }
        public OrderStatus Status { get; set; }
        public string Currency { get; set; }
        public decimal TotalSum { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}