namespace GridDomain.Domain.Tests.ProcessingWait
{
    internal interface IStopCondition
    {
        bool IsMeet(object msg);
    }
}