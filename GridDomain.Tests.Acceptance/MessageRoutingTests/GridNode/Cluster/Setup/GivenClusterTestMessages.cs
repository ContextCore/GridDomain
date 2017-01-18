using System;
using System.Linq;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup
{
    internal class GivenClusterTestMessages : IGivenMessages<ClusterEvent>
    {
        private readonly int _number;

        public GivenClusterTestMessages(int number)
        {
            _number = number;
        }

        public ClusterEvent[] GetCommands()
        {
            var guid = Guid.NewGuid();
            var commands =
                Enumerable.Range(0, _number)
                    .Select(n => new ClusterEvent {CorrelationId = guid, ExecuteOrder = n})
                    .ToArray();

            return commands;
        }
    }
}