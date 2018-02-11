using System;

namespace GridDomain.Common
{
    public static class MessageMetadataExtensions
    {
        public static MessageMetadata CreateChild(this IMessageMetadata metadata,
                                                  string messageId,
                                                  params ProcessEntry[] process)
        {
            return MessageMetadata.CreateFrom(messageId, metadata, process);
        }

        public static MessageMetadata CreateChild(this IMessageMetadata metadata,
                                                  IHaveId message,
                                                  params ProcessEntry[] process)
        {
            return metadata.CreateChild(message.Id, process);
        }

        public static MessageMetadata CreateChild(this IMessageMetadata metadata,
                                                  string messageId,
                                                  string who,
                                                  string what,
                                                  string why)
        {
            return CreateChild(metadata, (string) messageId, new ProcessEntry(who, what, why));
        }
    }
}