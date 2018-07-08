using System;
using System.Reflection;
using DotNetty.Codecs.Base64;

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
}