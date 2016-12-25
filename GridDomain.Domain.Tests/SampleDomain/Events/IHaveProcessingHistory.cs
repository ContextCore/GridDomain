using System;

namespace GridDomain.Tests.SampleDomain.Events
{
    public interface IHaveProcessingHistory
    {
        ProcessedHistory History { get; }

        Guid SourceId { get; }
    }
}