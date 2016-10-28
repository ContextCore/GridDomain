using System;
using System.Linq;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public static class WaitResultsExtensions
    {
        public static TMsg Message<TMsg>(this IWaitResults res, Predicate<TMsg> selector = null)
        {
            var sel = selector ?? (m => true);
            return res.All.OfType<TMsg>().FirstOrDefault(t => sel(t));
        }
    }
}