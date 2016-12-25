using System;

namespace GridDomain.Tests.Unit.SampleDomain.Events
{
    public interface IHaveProcessingHistory
    {
        ProcessedHistory History { get; }

        Guid SourceId { get; }
    }
}