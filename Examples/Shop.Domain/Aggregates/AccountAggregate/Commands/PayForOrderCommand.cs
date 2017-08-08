using System;
using NMoneys;

namespace Shop.Domain.Aggregates.AccountAggregate.Commands
{
    public class PayForOrderCommand : ChargeAccountCommand
    {
        public PayForOrderCommand(Guid accountId, Money amount, Guid orderId) : base(accountId, amount)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }
}