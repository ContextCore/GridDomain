using System;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    /// <summary>
    /// Works with messages sent in metadata envelop
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //really important will we wait for envelop types with local waiting and EventBus
    //or it will be distributed pub sub with exact topics
    public class MetadataConditionBuilder<T> : ConditionBuilder<T>
    {
        protected override bool DefaultFilter<TMsg>(object received)
        {
            return received.IsEnvelopedOfType<TMsg>();
        }

        protected override bool DomainFilterAdapter<TMsg>(object receivedMessage, Predicate<TMsg> domainMessageFilter)
        {
            return receivedMessage.IsEnvelopedOfTypeWithPredicate(domainMessageFilter);
        }
    }
    
    public static class ConditionsExtensions
    {
        public static bool IsEnvelopedOfType<TMsg>(this object msg)
        {
            return (msg as IMessageMetadataEnvelop)?.Message is TMsg;
        }

        public static bool IsEnvelopedOfTypeWithPredicate<TMsg>(this object receivedMessage, Predicate<TMsg> domainMessageFilter)
        {
            return (receivedMessage as IMessageMetadataEnvelop)?.Message is TMsg msg && domainMessageFilter(msg);
        }

        public static bool IsEnvelopedOfTypeWithCorrelation<TMsg>(this object message, string correlation)
        {
            if (message is IMessageMetadataEnvelop envelop)
                return envelop.Message is TMsg && envelop.Metadata.CorrelationId == correlation;
            return false;
        }
    }
    
    //really important will we wait for envelop types with local waiting and EventBus
    //or it will be distributed pub sub with exact topics
    public abstract class CorrelationConditionBuilder<T> : ConditionBuilder<T>
    {
        private readonly string _correlationId;

        protected CorrelationConditionBuilder(string correlationId)
        {
            _correlationId = correlationId;
        }

        protected override bool DomainFilterAdapter<TMsg>(object receivedMessage, Predicate<TMsg> domainMessageFilter)
        {
            return receivedMessage.IsEnvelopedOfTypeWithPredicate(domainMessageFilter); 
        }
        
        protected override bool DefaultFilter<TMsg>(object message)
        {
            return message.IsEnvelopedOfTypeWithCorrelation<TMsg>(_correlationId);
        }
    }

    public class LocalCorrelationConditionBuilder<T> : CorrelationConditionBuilder<T>
    {
        protected override void AddFilter(Type type, Func<object, bool> filter)
        {
            base.AddFilter(typeof(MessageMetadataEnvelop), filter);
        }

        public LocalCorrelationConditionBuilder(string correlationId) : base(correlationId) { }
    }
    
    public class LocalMetadataConditionBuilder<T> : MetadataConditionBuilder<T>
    {
        protected override void AddFilter(Type type, Func<object, bool> filter)
        {
            base.AddFilter(typeof(MessageMetadataEnvelop), filter);
        }
    }
}