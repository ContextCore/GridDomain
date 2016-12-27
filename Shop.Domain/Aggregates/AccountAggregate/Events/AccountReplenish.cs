using System;
using NMoneys;

namespace Shop.Domain.Aggregates.AccountAggregate.Events
{
    public class AccountReplenish : AccountAmountChanged
    {
        public AccountReplenish(Guid sourceId, Money amount) : base(sourceId, amount)
        {
        }
    }
}