using System;

namespace GridDomain.Tests.XUnit.SampleDomain.Events
{
    public interface IHaveProcessingHistory
    {
        ProcessedHistory History { get; }

        Guid SourceId { get; }
    }
}