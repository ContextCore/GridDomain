using System;
using System.Reflection;
using DotNetty.Codecs.Base64;
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
            return base.CheckMessageType(receivedMessage.SafeUnenvelope(), t, domainMessageFilter);
        }
    }

    //really important will we wait for envelop types with local waiting and EventBus
    //or it will be distributed pub sub with exact topics
    public class CorrelationConditionBuilder : MetadataEnvelopConditionBuilder
    {
        protected readonly string CorrelationId;
    
        protected CorrelationConditionBuilder(string correlationId)
        {
            CorrelationId = correlationId;
        }
    
        protected override Func<object, bool> AddFilter(Type messageType, Func<object, bool> filter = null)
        {
            bool CorrelationFilter(object m) => m.SafeCheckCorrelation(CorrelationId)
                                                && CheckMessageType(m, messageType, filter);
    
            base.AddFilter(messageType, CorrelationFilter);
    
            return CorrelationFilter;
        }
    }

    public static class ObjectExtensions
    {
        public static object SafeUnenvelope(this object msg)
        {
            return (msg as IMessageMetadataEnvelop)?.Message;
        }

        public static bool SafeCheckCorrelation(this object msg, string correlationId)
        {
            return (msg as IMessageMetadataEnvelop)?.Metadata?.CorrelationId == correlationId;
        }
    }

    public class LocalMetadataEnvelopConditionBuilder : MetadataEnvelopConditionBuilder
    {
        protected override Func<object, bool> AddFilter(Type messageType, Func<object, bool> filter = null)
        {
            AcceptedMessageTypes.Add(typeof(MessageMetadataEnvelop));

            bool FilterWithAdapter(object o) => CheckMessageType(o, messageType, filter);
            MessageFilters.Add(FilterWithAdapter);
            return FilterWithAdapter;
        }
    }
}