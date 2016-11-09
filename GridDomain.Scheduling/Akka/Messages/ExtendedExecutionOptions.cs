using System;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ExtendedExecutionOptions : ExecutionOptions
    {
        public Guid SuccessSuccessMessageId { get; }
        public string MessageIdFieldName { get; }

        public ExtendedExecutionOptions(DateTime runAt, Type successEventType, Guid successMessageId, string messageIdFieldName, TimeSpan? timeout = null) : base(runAt, successEventType, timeout)
        {
            SuccessSuccessMessageId = successMessageId;
            MessageIdFieldName = messageIdFieldName;
        }
    }
}