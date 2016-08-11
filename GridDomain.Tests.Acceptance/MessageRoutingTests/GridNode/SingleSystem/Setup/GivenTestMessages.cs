using System;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    internal class GivenTestMessages : IGivenMessages<TestMessage>
    {
        public TestMessage[] GetCommands()
        {
            var guid = Guid.NewGuid();
            var count = 0;

            var commands = new[]
            {
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count},
                new TestMessage {CorrelationId = guid, ExecuteOrder = ++count}
            };
            return commands;
        }
    }
}