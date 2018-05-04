using System;
using System.Reflection;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    /// <summary>
    /// Works with messages sent in metadata envelop
    /// </summary>
    //really important will we wait for envelop types with local waiting and EventBus
    //or it will be distributed pub sub with exact topics
    public class MetadataEnvelopConditionBuilder : ConditionBuilder
    {
        protected override bool CheckMessageType(object receivedMessage, Type t, Func<object, bool> domainMessageFilter = null)
        {
            var message = (receivedMessage as IMessageMetadataEnvelop)?.Message;
            
            if (t.IsInstanceOfType(message) )
            {
                if(domainMessageFilter == null)
                    return true;
                        
                return domainMessageFilter(message);
            }

            return false;
        }
    }
    
    //really important will we wait for envelop types with local waiting and EventBus
    //or it will be distributed pub sub with exact topics
    public abstract class CorrelationConditionFactory : ConditionBuilder
    {
        private readonly string _correlationId;

        protected CorrelationConditionFactory(string correlationId)
        {
            _correlationId = correlationId;
        }

        protected override Func<object,bool> AddFilter(Type messageType, Func<object, bool> filter=null)
        {
            bool Filter(object m)
            {
                if (m is IMessageMetadataEnvelop env)
                {
                    if (messageType.IsInstanceOfType(env.Message) && env.Metadata.CorrelationId == _correlationId)
                    {
                        if (filter == null) return true;
                        return filter(m);
                    }
                }

                return false;
            }

            base.AddFilter(messageType, Filter);
            
            return Filter;
        }
    }

    public class LocalCorrelationConditionFactory : CorrelationConditionFactory
    {
        protected override Func<object,bool> AddFilter(Type messageType, Func<object, bool> filter)
        {
            return base.AddFilter(typeof(MessageMetadataEnvelop), filter);
        }

        public LocalCorrelationConditionFactory(string correlationId) : base(correlationId) { }
    }
    
    public class LocalMetadataEnvelopConditionBuilder : MetadataEnvelopConditionBuilder
    {
        protected override Func<object,bool> AddFilter(Type messageType, Func<object, bool> filter)
        {
            AcceptedMessageTypes.Add(typeof(MessageMetadataEnvelop));
            bool FilterWithAdapter(object o) => CheckMessageType(o, messageType, filter);
            MessageFilters.Add(FilterWithAdapter);
            return FilterWithAdapter;
        }
    }
}