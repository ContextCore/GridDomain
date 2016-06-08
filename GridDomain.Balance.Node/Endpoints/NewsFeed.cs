using System;
using System.Collections.Generic;
using BusinessNews.Node.Endpoints.Contracts;
using GridDomain.CQRS.Quering;

namespace BusinessNews.Node.Endpoints
{
    public class NewsFeed : INewsFeed
    {
        //readmodel access
        private readonly IQuery<Guid, string> _newsQueryByBusinessId;
        private readonly IQuery<Guid, string> _insiderNewsQuery;

        public NewsFeed(IQuery<Guid,string> newsQueryByBusinessId, IQuery<Guid,string> insiderNewsQuery)
        {
            _insiderNewsQuery = insiderNewsQuery;
            _newsQueryByBusinessId = newsQueryByBusinessId;
        }

        public IReadOnlyCollection<string> GetNews(Guid businessId, SubscriptionClaims subscription)
        {
            if (!SecurityDummy.CanAccess<INewsFeed>(nameof(INewsFeed.GetNews), subscription.Claims))
            {
                string extendedSubscriptionPurchaseUrl;
                var claimsNeed = SecurityDummy.ListRequiredClaims<INewsFeed>(nameof(INewsFeed.GetNews));
                if (!SubscriptioPurchaseAdviserDummy.TryGetPurchaseURL(businessId, claimsNeed, out extendedSubscriptionPurchaseUrl))
                {
                    ResponseDummy.Redirect("Forbidden");
                }
                ResponseDummy.Redirect(extendedSubscriptionPurchaseUrl);
            }

            return _newsQueryByBusinessId.Execute(businessId);
        }

        public IReadOnlyCollection<string> GetInsiderNews(Guid businessId, SubscriptionClaims subscription)
        {
            if (!SecurityDummy.CanAccess<INewsFeed>(nameof(INewsFeed.GetInsiderNews), subscription.Claims))
            {
                string extendedSubscriptionPurchaseUrl;
                var claimsNeed = SecurityDummy.ListRequiredClaims<INewsFeed>(nameof(INewsFeed.GetInsiderNews));
                if (!SubscriptioPurchaseAdviserDummy.TryGetPurchaseURL(businessId, claimsNeed, out extendedSubscriptionPurchaseUrl))
                {
                    ResponseDummy.Redirect("Forbidden");
                }
                ResponseDummy.Redirect(extendedSubscriptionPurchaseUrl);
            }
            return _insiderNewsQuery.Execute(businessId);
        }
    }
}