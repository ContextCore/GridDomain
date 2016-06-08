using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessNews.Node.Endpoints.Contracts
{
    /// <summary>
    /// Web-exposed endpoint for managing business subscriptions.
    /// Called from UI or external system
    /// </summary>
    public interface ISubscriptionService
    {
        Task SubscribeBusiness(Guid businessId, Guid offerId);
        Task<IReadOnlyCollection<OfferViewModel>> ListSubscriptions();
        Task<SubscriptionViewModel> GetBusinessSubscription(Guid businessId);
    }
}