using System;
using GridDomain.EventSourcing.Sagas;

namespace BusinessNews.Domain.Sagas.BuySubscription
{
    internal class AccountRememberedEvent : SagaStateEvent
    {
        public AccountRememberedEvent(Guid accountId, Guid businessId, Guid suibscriptionId, Guid sagaId) : base(sagaId)
        {
            AccountId = accountId;
            BusinessId = businessId;
            SuibscriptionId = suibscriptionId;
        }

        public Guid AccountId { get; }
        public Guid BusinessId { get; }
        public Guid SuibscriptionId { get; }
    }
}