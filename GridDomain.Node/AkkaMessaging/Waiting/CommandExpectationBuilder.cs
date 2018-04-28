using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Transport;

namespace GridDomain.Node.AkkaMessaging.Waiting
{


    public class CommandExpectationBuilder<TCommand> : MessagesWaiter<Task<IWaitResult>>,
                                                       ICommandExpectationBuilder where TCommand : ICommand
    {
        private readonly ConditionCommandExecutor<TCommand> _executor;

        public CommandExpectationBuilder(TCommand command,
                             IMessageMetadata commandMetadata,
                             ActorSystem system,
                             IActorTransport transport,
                             ICommandExecutor executor,
                             TimeSpan defaultTimeout) : this(system, transport, defaultTimeout, 
                                                             new ConditionCommandExecutor<TCommand>(command, commandMetadata, executor)) { }

        public CommandExpectationBuilder(ActorSystem system,
                             IActorSubscriber subscriber,
                             TimeSpan defaultTimeout,
                             ConditionCommandExecutor<TCommand> executor) : base(system, subscriber, defaultTimeout, executor.ConditionFactory)
        {
            _executor = executor;
            _executor.ConditionFactory.CreateResultFunc = Start;
        }


       IConditionCommandExecutor<TMsg> ICommandExpectationBuilder.Expect<TMsg>(Predicate<TMsg> filter) 
       {
           Expect(filter);
           return new ConditionCommandExecutorTypedDecorator<TMsg>(_executor);
       }

        public IConditionCommandExecutor Expect(Type type, Func<object, bool> filter = null)
        {
            ConditionFactory.And(type, filter);
            return _executor;
        }
    }
}