using System;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.ReadModel
{
    public class BalanceChangeProjectedNotification
    {
        public BalanceChangeProjectedNotification(Guid balanceId)
        {
            BalanceId = balanceId;
        }

        public Guid BalanceId { get; set; }
    }
}