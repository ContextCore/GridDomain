using System;
using NMoneys;

namespace Shop.Domain.Aggregates.AccountAggregate.Events
{
    public class AccountReplenish : AccountAmountChanged
    {
        public AccountReplenish(Guid sourceId, Guid changeId, Money amount) : base(sourceId, changeId, amount) {}
    }
}