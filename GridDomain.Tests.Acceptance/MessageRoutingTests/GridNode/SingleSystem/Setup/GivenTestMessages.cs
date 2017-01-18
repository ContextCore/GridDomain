using System;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup
{
    internal class GivenTestMessages : IGivenMessages<TestEvent>
    {
        public TestEvent[] GetCommands()
        {
            var guid = Guid.NewGuid();
            var count = 0;

            var commands = new[]
            {
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count},
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count},
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count},
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count},
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count},
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count},
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count},
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count},
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count},
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count},
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count},
                new TestEvent {CorrelationId = guid, ExecuteOrder = ++count}
            };
            return commands;
        }
    }
}