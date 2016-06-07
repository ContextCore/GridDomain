using System;

namespace BusinessNews.Domain
{
    public interface ISubscriptionStore
    {
        Subscription GetFreeSubscription(Guid businessId);
        Subscription GetPaidSubscription(Guid businessId);
    }
}