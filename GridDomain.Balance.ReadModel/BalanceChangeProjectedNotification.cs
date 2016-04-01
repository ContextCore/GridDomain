using System;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.ReadModel
{
    public class BalanceChangeProjectedNotification: ISourcedEvent
    {
        public Guid BalanceId { get;}
        public Guid SourceId { get; }
        public Guid SagaId { get; set; }
        public DateTime CreatedTime { get; }

        public BalanceChangeProjectedNotification(Guid balanceId, ISourcedEvent e)
        {
            BalanceId = balanceId;
            SourceId = e.SourceId;
            SagaId = e.SagaId;
            CreatedTime = e.CreatedTime;
        }
    }
}