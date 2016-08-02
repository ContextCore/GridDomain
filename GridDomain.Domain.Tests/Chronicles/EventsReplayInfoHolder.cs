using System;
using System.Collections.Generic;
using GridDomain.Tests.SampleDomain.Events;

namespace GridDomain.Tests.Chronicles
{
    public static class EventsReplayInfoHolder
    {
        public static Dictionary<Guid, List<ProcessedHistory>> ProcessedMessages =new Dictionary<Guid, List<ProcessedHistory>>();
    }
}