using System;

namespace GridDomain.Balance.Domain.BalanceAggregate
{
    public class BalanceNotFoundException : Exception
    {
        public BalanceNotFoundException(Guid balanceId, Guid cmd) :
            base($"Cannot find balance {balanceId}, requested by command {cmd}")
        {
        }
    }
}