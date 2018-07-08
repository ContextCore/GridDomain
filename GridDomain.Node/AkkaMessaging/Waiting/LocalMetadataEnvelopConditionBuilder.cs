using System;
using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging.Waiting {
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