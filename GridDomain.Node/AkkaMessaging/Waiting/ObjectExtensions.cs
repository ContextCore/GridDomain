using GridDomain.Common;

namespace GridDomain.Node.AkkaMessaging.Waiting {
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
}