namespace GridDomain.Scheduling.Akka.Messages
{
    public interface IMessageProcessingStatusChanged
    {
        string TaskId { get; }
    }
}