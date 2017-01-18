using System;

namespace GridDomain.Common
{
    public class MessageMetadata : IMessageMetadata
    {

        public Guid MessageId { get; }
        public Guid CasuationId { get; }
        public Guid CorrelationId { get; }
        public DateTime Created { get; }
        public ProcessHistory History { get; }

        public MessageMetadata(Guid messageId, DateTime? created = null, Guid? correlationId = null, Guid? casuationId = null, ProcessHistory  history = null)
        {
            MessageId = messageId;
            Created = created ?? BusinessDateTime.UtcNow;
            CasuationId = casuationId ?? Guid.Empty;
            CorrelationId = correlationId ?? Guid.Empty;
            History = history ?? new ProcessHistory(null);
        }


        public static MessageMetadata CreateFrom(Guid messageId, 
                                                 IMessageMetadata existedMessage, 
                                                 params ProcessEntry[] process)
        {
            return new MessageMetadata(messageId, 
                                       BusinessDateTime.UtcNow, 
                                       existedMessage.CorrelationId, 
                                       existedMessage.MessageId,
                                       new ProcessHistory(process));
        }

        public static MessageMetadata Empty { get; } = new MessageMetadata(Guid.Empty, BusinessDateTime.UtcNow);
    }
}