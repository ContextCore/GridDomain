using System;
using System.Linq;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public static class WaitResultsExtensions
    {
        public static TMsg Message<TMsg>(this IWaitResults res, Predicate<TMsg> selector = null) where TMsg: class
        {
            var sel = selector ?? (m => true);
            var msg = res.All.OfType<TMsg>().FirstOrDefault(t => sel(t));
            return msg ?? MessageWithMetadata(res, selector)?.Message;
        }

        public static IMessageMetadataEnvelop<TMsg> MessageWithMetadata<TMsg>(this IWaitResults res, Predicate<TMsg> selector = null)
        {
            var sel = selector ?? (m => true);
            return res.All.OfType<IMessageMetadataEnvelop<TMsg>>().FirstOrDefault(t => sel(t.Message));
        }
    }
}