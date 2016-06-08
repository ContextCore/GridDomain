using System;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Commands
{
    public class ReplenishAccountByCardCommand : ChargeAccountCommand
    {
        public readonly CreditCardInfo CreditCard;

        public ReplenishAccountByCardCommand(Guid accountId, Money amount, CreditCardInfo creditCard) : base(accountId, amount)
        {
            CreditCard = creditCard;
        }
    }
}