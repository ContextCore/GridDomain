using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class CommandWaiter<TCommand> : LocalMessagesWaiter<Task<IWaitResults>>,
                                           ICommandWaiter where TCommand : ICommand
    {
        private readonly CommandConditionBuilder<TCommand> _conditionBuilder;

        public CommandWaiter(TCommand command,
                             IMessageMetadata commandMetadata,
                             ActorSystem system,
                             IActorTransport transport,
                             IActorRef executorActor,
                             TimeSpan defaultTimeout) : this(system, transport, defaultTimeout, new CommandConditionBuilder<TCommand>(command, commandMetadata, executorActor)) { }

        private CommandWaiter(ActorSystem system,
                             IActorSubscriber subscriber,
                             TimeSpan defaultTimeout,
                             CommandConditionBuilder<TCommand> conditionBuilder) : base(system, subscriber, defaultTimeout, conditionBuilder)
        {
            _conditionBuilder = conditionBuilder;
            _conditionBuilder.CreateResultFunc = Start;
        }


        public Task<IWaitResults> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true)
        {
            return _conditionBuilder.Execute(timeout, failOnAnyFault);
        }

        ICommandConditionBuilder ICommandWaiter.Expect<TMsg>(Predicate<TMsg> filter)
        {
            return _conditionBuilder.And(filter);
        }

        ICommandConditionBuilder ICommandWaiter.Expect(Type type, Func<object, bool> filter)
        {
            return (ICommandConditionBuilder) _conditionBuilder.And(type, filter);
        }
    }
}