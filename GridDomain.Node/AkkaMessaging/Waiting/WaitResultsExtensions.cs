using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public static class WaitResultsExtensions
    {
        public static TMsg Message<TMsg>(this IWaitResult res, Predicate<TMsg> selector = null) where TMsg : class
        {
            var sel = selector ?? (m => true);
            var msg = res.All.OfType<TMsg>().FirstOrDefault(t => sel(t));
            return msg ?? MessageWithMetadata(res, selector)?.Message;
        }
        
        public static async Task<TMsg> Message<TMsg>(this Task<IWaitResult> res, Predicate<TMsg> selector = null) where TMsg : class
        {
            return (await res).Message<TMsg>(selector);
        }
        
        public static async Task<TMsg> Message<TMsg>(this Task<IWaitResult<TMsg>> res, Predicate<TMsg> selector = null) where TMsg : class
        {
            return (await res).Message<TMsg>(selector);
        }

        public static IMessageMetadataEnvelop<TMsg> MessageWithMetadata<TMsg>(this IWaitResult res,
                                                                              Predicate<TMsg> selector = null)
        {
            var sel = selector ?? (m => true);
            var expectedMessages = res.All.OfType<IMessageMetadataEnvelop>().Where(t => t.Message is TMsg).ToArray();
                if(!expectedMessages.Any())
                    throw new InvalidOperationException($"{typeof(IMessageMetadataEnvelop)} messages with expected type {typeof(TMsg)} were not received");
                        
             
            var resA =  expectedMessages.FirstOrDefault(t => sel((TMsg)t.Message));
            if (resA == null)
                throw new InvalidOperationException("Received messages do not satisfy filter");
            
            return new MessageMetadataEnvelop<TMsg>((TMsg)resA.Message, resA.Metadata);
        }
    }
}