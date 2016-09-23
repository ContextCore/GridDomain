using System;
using GridDomain.Tests.SampleDomain.Events;

namespace GridDomain.Tests.SampleDomain.ProjectionBuilders
{
    public class MessageHandleException : Exception
    {
        public readonly SampleAggregateChangedEvent Msg;

        public MessageHandleException(SampleAggregateChangedEvent msg)
        {
            Msg = msg;
        }
    }
}