using System;
using GridDomain.EventSourcing;

namespace BusinessNews.ReadModel
{
    public class BusinessBalanceCreatedProjectedNotification : ISourcedEvent
    {
        public Guid BalanceId;

        public BusinessBalanceCreatedProjectedNotification(Guid balanceId, ISourcedEvent o)
        {
            BalanceId = balanceId;
            SourceId = o.SourceId;
            SagaId = o.SagaId;
            CreatedTime = o.CreatedTime;
        }

        public Guid SourceId { get; }
        public Guid SagaId { get; set; }
        public DateTime CreatedTime { get; }
        public int Version { get; }
    }
}