namespace GridDomain.Tests.SampleDomain.Events
{
    public interface IHaveProcessingHistory
    {
        ProcessedHistory History { get; }
    }
}