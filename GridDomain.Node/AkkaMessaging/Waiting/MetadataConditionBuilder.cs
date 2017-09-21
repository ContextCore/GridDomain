using System;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    /// <summary>
    /// Works with messages sent in metadata envelop
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MetadataConditionBuilder<T> : ConditionBuilder<T>
    {

        protected override bool DefaultFilter<TMsg>(object received)
        {
            return (received as IMessageMetadataEnvelop)?.Message is TMsg;
        }

        protected override bool FilterDecorator<TMsg>(object receivedMessage, Predicate<TMsg> domainMessageFilter)
        {
            var msg = (receivedMessage as IMessageMetadataEnvelop)?.Message as TMsg;
            return msg != null && domainMessageFilter(msg);
        }

        protected override void AddFilter(Type type, Func<object, bool> filter)
        {
            base.AddFilter(typeof(MessageMetadataEnvelop), filter);
        }
    }
}