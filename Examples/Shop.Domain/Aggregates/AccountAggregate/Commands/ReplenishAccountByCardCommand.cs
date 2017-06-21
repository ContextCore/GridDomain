using System;
using NMoneys;

namespace Shop.Domain.Aggregates.AccountAggregate.Commands
{
    public class ReplenishAccountByCardCommand : ChargeAccountCommand
    {
        public readonly string CreditCard;

        public ReplenishAccountByCardCommand(Guid accountId, Money amount, string creditCard) : base(accountId, amount)
        {
            CreditCard = creditCard;
        }
    }
}