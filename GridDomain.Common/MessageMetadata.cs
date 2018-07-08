namespace GridDomain.Common
{
    public class MessageMetadata : IMessageMetadata
    {
        public MessageMetadata(string messageId,
                               string correlationId = null,
                               string casuationId = null,
                               ProcessHistory history = null)
        {
            MessageId = messageId;
            CasuationId = casuationId;
            CorrelationId = correlationId ?? messageId;
            History = history ?? new ProcessHistory(null);
        }


        public static MessageMetadata New(string messageId,
                                          string correlationId = null,
                                          string casuationId = null)
        {
            return new MessageMetadata(messageId, correlationId, casuationId);
        }
        public static MessageMetadata Empty { get; } = new MessageMetadata("","");

        public string MessageId { get; }
        public string CasuationId { get; }
        public string CorrelationId { get; }
        public ProcessHistory History { get; }

        public static MessageMetadata CreateFrom(string messageId,
                                                 IMessageMetadata existedMessage,
                                                 params ProcessEntry[] process)
        {
            return new MessageMetadata(messageId,
                                       existedMessage.CorrelationId,
                                       existedMessage.MessageId,
#if DEBUG
                
                new ProcessHistory(process)
#endif
#if RELEASE
                null
#endif
            );

        }
    }
}