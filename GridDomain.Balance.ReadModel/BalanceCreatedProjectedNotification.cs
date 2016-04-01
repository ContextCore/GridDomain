using System;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.ReadModel
{
    public class BalanceCreatedProjectedNotification : ISourcedEvent
    {
        public Guid BalanceId;

        public BalanceCreatedProjectedNotification(Guid balanceId, ISourcedEvent o)
        {
            BalanceId = balanceId;
            SourceId = o.SourceId;
            SagaId = o.SagaId;
            CreatedTime = o.CreatedTime;
        }

        public Guid SourceId { get; }
        public Guid SagaId { get; set; }
        public DateTime CreatedTime { get; }
    }
}