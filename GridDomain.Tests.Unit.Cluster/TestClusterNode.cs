using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.CommandPipe;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Transport;
using Serilog;

namespace GridDomain.Tests.Unit.Cluster
{
    public class TestClusterNode : ITestGridDomainNode
    {
        private TestKit _testKit;
        public IExtendedGridDomainNode Node { get; }

        public TestClusterNode(GridClusterNode node, TestKit kit)
        {
            _testKit = kit;
            Node = node;
        }

        public Task Execute<T>(T command, IMessageMetadata metadata = null, CommandConfirmationMode confirm = CommandConfirmationMode.Projected) where T : ICommand
        {
            return Node.Execute(command, metadata, confirm);
        }

        public ICommandExpectationBuilder Prepare<U>(U cmd, IMessageMetadata metadata = null) where U : ICommand
        {
            return Node.Prepare(cmd, metadata);
        }

        public IMessageWaiter NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            return Node.NewExplicitWaiter(defaultTimeout);
        }

        public IMessageWaiter NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return Node.NewWaiter(defaultTimeout);
        }

        public void Dispose()
        {
            Node.Dispose();
        }

        public ActorSystem System => Node.System;

        public TimeSpan DefaultTimeout => Node.DefaultTimeout;
        public IActorTransport Transport => Node.Transport;

        public IActorCommandPipe Pipe => Node.Pipe;

        public Task Start()
        {
            return Node.Start();
        }

        public Task Stop()
        {
            return Node.Stop();
        }

        public ILogger Log => Node.Log;
        public EventsAdaptersCatalog EventsAdaptersCatalog => Node.EventsAdaptersCatalog;

        public async Task<T> LoadAggregateByActor<T>(string id) where T : Aggregate
        {
            var name = EntityActorName.New<T>(id)
                                      .ToString();

            var actor = await _testKit.LoadActor<ClusterAggregateActor<T>>(name);

            return actor.State;
        }

        public async Task<TState> LoadProcess<TState>(string id) where TState : class, IProcessState
        {
            var name = EntityActorName.New<ProcessStateAggregate<TState>>(id)
                                      .ToString();

            var actor = await _testKit.LoadActor<ClusterProcessStateActor<TState>>(name);

            return actor.State.State;
        }

        public IProcessManagerExpectationBuilder PrepareForProcessManager(DomainEvent msg, MessageMetadata metadata = null)
        {
            throw new NotImplementedException();
        }

        public IProcessManagerExpectationBuilder PrepareForProcessManager(IFault msg, MessageMetadata metadata = null)
        {
            throw new NotImplementedException();
            // var res = await NewClusterDebugWaiter(Node,timeout)
            //                 .Expect<TExpect>()
            //                 .Create()
            //                 .SendToProcessManagers<TExpect>(msg);
            //
            // return res.Message<TExpect>();
        }

        static IMessageWaiter NewClusterDebugWaiter(IExtendedGridDomainNode node, TimeSpan? timeout = null)
        {
            var conditionBuilder = new MetadataEnvelopConditionBuilder();
            var conditionFactory = new MessageConditionFactory<Task<IWaitResult>>(conditionBuilder);
            var waiter = new MessagesWaiter(node.System, node.Transport, timeout ?? node.DefaultTimeout, conditionFactory);
            return waiter;
        }

      
        public IMessageWaiter NewTestWaiter(TimeSpan? timeout = null)
        {
            return NewClusterDebugWaiter(Node, timeout);
        }
    }
}