using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Transport;
using Serilog;

namespace GridDomain.Tests.Unit {
    public class TestLocalNode : ITestGridDomainNode
    {
        private TestKit _testKit;
        public IExtendedGridDomainNode Node { get; }

        public TestLocalNode(IExtendedGridDomainNode node, TestKit kit)
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
            var actor = await _testKit.LoadActor<AggregateActor<T>>(name);

            return actor.State;
        }
                                                                  
        public async Task<TState> LoadProcess<TState>(string id) where TState : class, IProcessState
        {
            return (await LoadAggregateByActor<ProcessStateAggregate<TState>>(id)).State;
        }

        public IProcessManagerExpectationBuilder PrepareForProcessManager(DomainEvent msg, MessageMetadata metadata = null)
        {
            
            return new ProcessManagerExpectationBuilder(new MessageMetadataEnvelop(msg,metadata ?? MessageMetadata.Empty), Node);

        }

        public IProcessManagerExpectationBuilder PrepareForProcessManager(IFault msg, MessageMetadata metadata = null)
        {
            return new ProcessManagerExpectationBuilder(new MessageMetadataEnvelop(msg,metadata ?? MessageMetadata.Empty), Node);
        }

        class ProcessManagerExpectationBuilder : IProcessManagerExpectationBuilder
        {
            private readonly IExtendedGridDomainNode _extendedGridDomainNode;
            private readonly object _msg;

            public ProcessManagerExpectationBuilder(object msg, IExtendedGridDomainNode node)
            {
                _msg = msg;
                _extendedGridDomainNode = node;
            }
            public IConditionedProcessManagerSender<TMsg> Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
            {
                var sender = new ConditionedProcessManagerSender<TMsg>(_extendedGridDomainNode,_msg,
                                                                       new MessageConditionFactory<Task<IWaitResult>>(new LocalMetadataEnvelopConditionBuilder()));
                sender.And<TMsg>(filter);
                return sender;
            }

            class ConditionedProcessManagerSender<T> : IConditionedProcessManagerSender<T> where T : class
            {
                private readonly MessageConditionFactory<Task<IWaitResult>> _messageConditionFactory;
                private readonly IExtendedGridDomainNode _node;
                private readonly object _msg;

                public ConditionedProcessManagerSender(IExtendedGridDomainNode node,
                                                       object msg,
                                                       MessageConditionFactory<Task<IWaitResult>> messageConditionFactory)
                {
                    _msg = msg;
                    _node = node;
                    _messageConditionFactory = messageConditionFactory;
                }

                async Task<IWaitResult<T>> IConditionedProcessManagerSender<T>.Send(TimeSpan? timeout, bool failOnAnyFault)
                {
                    return WaitResult.Parse<T>(await Send(timeout, failOnAnyFault));
                }

                public IConditionedProcessManagerSender And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
                {
                    _messageConditionFactory.And(filter);
                    return this;
                }

                public IConditionedProcessManagerSender Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
                {
                    _messageConditionFactory.Or(filter);
                    return this;
                }


                public async Task<IWaitResult> Send(TimeSpan? timeout = null, bool failOnAnyFault = true)
                {
                    var defaultTimeout = timeout ?? _node.DefaultTimeout;
                    
                    var waiter = new MessagesWaiter(_node.System, _node.Transport, defaultTimeout, _messageConditionFactory);
                    var results = waiter.Start();

                     _node.Pipe.ProcessesPipeActor.Tell(_msg);

                    return await results;
                }
            }
        }
        


        //static IMessageWaiter NewLocalDebugWaiter(IExtendedGridDomainNode node, TimeSpan? timeout = null)
        //{
        //    var conditionBuilder = new LocalMetadataEnvelopConditionBuilder();
        //    var conditionFactory = new MessageConditionFactory<Task<IWaitResult>>(conditionBuilder);
        //    var waiter = new MessagesWaiter(node.System, node.Transport, timeout ?? node.DefaultTimeout, conditionFactory);
        //    return waiter;
        //}
    }
}