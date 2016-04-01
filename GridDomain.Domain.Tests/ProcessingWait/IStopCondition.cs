namespace GridDomain.Domain.Tests.ProcessingWait
{
    interface IStopCondition
    {
        bool IsMeet(object msg);
    }
}