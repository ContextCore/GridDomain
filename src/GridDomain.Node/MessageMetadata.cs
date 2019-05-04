namespace GridDomain.Node
{
    public class MessageMetadata : IMessageMetadata
    {
        public MessageMetadata(string messageId,
                               string correlationId = null,
                               string casuationId = null)
        {
            MessageId = messageId;
            CasuationId = casuationId;
            CorrelationId = correlationId ?? messageId;
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

        public static MessageMetadata CreateFrom(string messageId,
                                                 IMessageMetadata existedMessage)
        {
            return new MessageMetadata(messageId,
                existedMessage.CorrelationId,
                existedMessage.MessageId);

        }
    }
}