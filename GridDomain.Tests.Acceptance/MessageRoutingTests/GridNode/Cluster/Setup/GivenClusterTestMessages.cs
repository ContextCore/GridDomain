using System;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup
{
    class GivenClusterTestMessages : IGivenCommands<ClusterMessage>
    {
        public ClusterMessage[] GetCommands()
        {
            var guid = Guid.NewGuid();
            int count = 0;

            var commands = new[]
            {
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count},
                new ClusterMessage() {CorrelationId = guid, ExecuteOrder = ++count}
            };
            return commands;
        }
    }
}