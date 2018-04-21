using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;

namespace GridDomain.Tests.Unit.Cluster {
    public static class GridNodeExtensions
    {
       
        public static async Task<TExpect> SendToProcessManager<TExpect>(this GridDomainNode node, DomainEvent msg, TimeSpan? timeout = null) where TExpect : class
        {
            var res = await node.NewClusterDebugWaiter(timeout)
                                .Expect<TExpect>()
                                .Create()
                                .SendToProcessManagers(msg);

            return res.Message<TExpect>();
        }

        public static async Task<TState> GetTransitedState<TState>(this GridDomainNode node, DomainEvent msg, TimeSpan? timeout = null) where TState : IProcessState
        {
            var res = await node.SendToProcessManager<ProcessReceivedMessage<TState>>(msg,timeout);
            return res.State;
        }

        public static async Task<TState> GetCreatedState<TState>(this GridDomainNode node, DomainEvent msg, TimeSpan? timeout = null) where TState : IProcessState
        {
            var res = await node.SendToProcessManager<ProcessManagerCreated<TState>>(msg, timeout);
            return res.State;
        }
        
        public static IMessageWaiter<AnyMessagePublisher> NewClusterDebugWaiter(this GridDomainNode node, TimeSpan? timeout = null)
        {
            var conditionBuilder = new MetadataConditionBuilder<AnyMessagePublisher>();
            var waiter = new MessagesWaiter<AnyMessagePublisher>(node.System, node.Transport, timeout ?? node.DefaultTimeout, conditionBuilder);
            conditionBuilder.CreateResultFunc = t => new AnyMessagePublisher(node.Pipe, waiter);
            return waiter;
        }
    }
}