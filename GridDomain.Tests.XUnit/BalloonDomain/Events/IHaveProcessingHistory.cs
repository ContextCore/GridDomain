using System;

namespace GridDomain.Tests.XUnit.BalloonDomain.Events
{
    public interface IHaveProcessingHistory
    {
        ProcessedHistory History { get; }

        Guid SourceId { get; }
    }
}