using System;
using System.Collections.Generic;

namespace BusinessNews.Domain
{
    public interface INewsFeed
    {
        IReadOnlyCollection<string> GetNews(Guid businessId,  Subscription subscription);
        IReadOnlyCollection<string> GetVipNews(Guid businessId, Subscription subscription);
    }
}