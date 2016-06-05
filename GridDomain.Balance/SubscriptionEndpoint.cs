using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMoneys;

namespace BusinessNews.Domain
{

    public interface INewsFeed
    {
        IReadOnlyCollection<string> GetNews(Guid businessId,  Subscription subscription);
        IReadOnlyCollection<string> GetVipNews(Guid businessId, Subscription subscription);
    }
    public interface ISubscriptionStore
    {
        Subscription GetFreeSubscription(Guid businessId);
        Subscription GetPaidSubscription(Guid businessId);

    }

    public interface IPaymentGate
    {
        void ReplenishAccount(Guid accountId, CreditCardInfo amount);
    }

    //public enum Subscription
    //{
    //    Basic,
    //    Vip
    //}
    public class CreditCardInfo
    {
        public string Secret;
    }

    public class Subscription
    {
        public Guid Id;
        public string[] claims;
    }

 

    class SubscriptionEndpoint
    {
    }
}
