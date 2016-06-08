using System;
using System.Collections.Generic;
using BusinessNews.Domain.OfferAggregate;

namespace BusinessNews.Node.Endpoints.Contracts
{
    /// <summary>
    /// Web-exposed service with consumables for customers 
    /// </summary>
    public interface INewsFeed
    {
        [Resource(WellKnownGrants.PublicNewsRead)]
        IReadOnlyCollection<string> GetNews(Guid businessId, SubscriptionClaims subscription);

        [Resource(WellKnownGrants.InsiderNewsRead)]
        IReadOnlyCollection<string> GetInsiderNews(Guid businessId, SubscriptionClaims subscription);
    }
}