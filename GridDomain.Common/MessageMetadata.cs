using System;

namespace GridDomain.Common
{
    public class MessageMetadata : IMessageMetadata
    {
        public MessageMetadata(Guid messageId, DateTime created, Guid? correlationId = null, Guid? casuationId = null, ProcessEntry[] history = null)
        {
            MessageId = messageId;
            Created = created;
            CasuationId = casuationId ?? Guid.Empty;
            CorrelationId = correlationId ?? Guid.Empty;
            History = new ProcessHistory(history);
        }

        public static MessageMetadata CreateFrom(Guid messageId, 
                                                 IMessageMetadata existedMessage, 
                                                 params ProcessEntry[] process)
        {
            return new MessageMetadata(messageId, 
                                       BusinessDateTime.UtcNow, 
                                       existedMessage.CorrelationId, 
                                       existedMessage.MessageId,
                                       process);
        }

        public Guid MessageId { get; }
        public Guid CasuationId { get; }
        public Guid CorrelationId { get; }
        public DateTime Created { get; }
        public ProcessHistory History { get;}

        public static MessageMetadata Empty()
        {
            return new MessageMetadata(Guid.Empty, BusinessDateTime.UtcNow);
        }
    }
}