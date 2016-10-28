using System;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ExtendedExecutionOptions : ExecutionOptions
    {
        public Guid SuccessMessageId { get; }
        public string MessageIdFieldName { get; }

        public ExtendedExecutionOptions(DateTime runAt, Type succesEventType, Guid messageId, string messageIdFieldName, TimeSpan timeout = new TimeSpan()) : base(runAt, succesEventType, timeout)
        {
            SuccessMessageId = messageId;
            MessageIdFieldName = messageIdFieldName;
        }
    }
}