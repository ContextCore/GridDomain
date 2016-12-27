using System;
using NMoneys;

namespace Shop.Domain.Aggregates.AccountAggregate.Events
{
    public class AccountWithdrawal : AccountAmountChanged
    {
        public AccountWithdrawal(Guid sourceId, Money amount) : base(sourceId, amount)
        {
        }
    }
}