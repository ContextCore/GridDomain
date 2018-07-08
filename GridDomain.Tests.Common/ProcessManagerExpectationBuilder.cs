using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.Common {
    public class ProcessManagerExpectationBuilder : IProcessManagerExpectationBuilder
    {
        private readonly IExtendedGridDomainNode _extendedGridDomainNode;
        private readonly object _msg;
        private readonly MessageConditionFactory<Task<IWaitResult>> _messageConditionFactory;

        public ProcessManagerExpectationBuilder(object msg, IExtendedGridDomainNode node, MessageConditionFactory<Task<IWaitResult>> messageConditionFactory)
        {
            _messageConditionFactory = messageConditionFactory;
            _msg = msg;
            _extendedGridDomainNode = node;
        }

        public IConditionedProcessManagerSender<TMsg> Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            var sender = new ConditionedProcessManagerSender<TMsg>(_extendedGridDomainNode,
                                                                   _msg,
                                                                   _messageConditionFactory);
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
}