using System;

namespace GridDomain.Tests.Unit.BalloonDomain.Events
{
    public interface IHaveProcessingHistory
    {
        ProcessedHistory History { get; }

        string SourceId { get; }
    }
}