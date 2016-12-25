using System;
using GridDomain.Tests.Unit.SampleDomain.Events;

namespace GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders
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