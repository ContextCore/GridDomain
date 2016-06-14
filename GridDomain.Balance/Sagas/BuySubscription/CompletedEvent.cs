using System;
using GridDomain.EventSourcing.Sagas;

namespace BusinessNews.Domain.Sagas.BuySubscription
{
    public class CompletedEvent : SagaStateEvent
    {
        public CompletedEvent(Guid sourceId): base(sourceId)
        {
        }
    }
}