using System;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ExtendedExecutionOptions : ExecutionOptions
    {
        public ExtendedExecutionOptions(DateTime runAt,
                                        Type succesEventType,
                                        Guid successMessageId,
                                        string messageIdFieldName,
                                        TimeSpan? timeout = null) : base(runAt, succesEventType, timeout)
        {
            SuccessMessageId = successMessageId;
            MessageIdFieldName = messageIdFieldName;
        }

        public Guid SuccessMessageId { get; }
        public string MessageIdFieldName { get; }
    }
}