using System;

namespace BusinessNews.ReadModel
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