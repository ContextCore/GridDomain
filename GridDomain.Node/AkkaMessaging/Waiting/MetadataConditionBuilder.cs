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
            return received is IMessageMetadataEnvelop<TMsg>;
        }

        protected override bool CustomFilter<TMsg>(object receivedMessage, Predicate<TMsg> domainMessageFilter)
        {
            var envelop = receivedMessage as IMessageMetadataEnvelop<TMsg>;
            return envelop != null && domainMessageFilter(envelop.Message);
        }

        protected override void AddFilter(Type type, Func<object, bool> filter)
        {
            base.AddFilter(MessageMetadataEnvelop.GenericForType(type), filter);
        }

       //private static bool MetadataAwareFilter(Func<object, bool> filter, object o)
       //{
       //    var envelop = o as IMessageMetadataEnvelop;
       //    return envelop != null && filter(envelop.Message);
       //}
    }
}