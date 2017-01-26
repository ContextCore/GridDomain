using System;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders
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