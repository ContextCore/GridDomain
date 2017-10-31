using System;
using System.CodeDom;

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
            CorrelationId = correlationId;
            History = history ?? new ProcessHistory(null);
        }


        public static MessageMetadata New(Guid messageId,
                                          Guid? correlationId = null,
                                          Guid? casuationId = null)
        {
            return new MessageMetadata(messageId.ToString("D"), correlationId?.ToString("D"), casuationId?.ToString("D"));
        }
        public static MessageMetadata Empty { get; } = new MessageMetadata("","");

        public string MessageId { get; }
        public string CasuationId { get; }
        public string CorrelationId { get; }
        public ProcessHistory History { get; }

        public static MessageMetadata CreateFrom(Guid messageId,
                                                 IMessageMetadata existedMessage,
                                                 params ProcessEntry[] process)
        {
            return new MessageMetadata(messageId.ToString("D"),
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