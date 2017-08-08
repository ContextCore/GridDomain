using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;

using GridDomain.Node.Transports;

namespace GridDomain.Node.AkkaMessaging.Waiting
{


    public class CommandWaiter<TCommand> : LocalMessagesWaiter<Task<IWaitResult>>,
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


       ICommandConditionBuilder<TMsg> ICommandWaiter.Expect<TMsg>(Predicate<TMsg> filter) 
       {
           Expect(filter);
           return new CommandConditionBuilderTypedDecorator<TMsg>(_conditionBuilder);
       }
       
       ICommandConditionBuilder ICommandWaiter.Expect(Type type, Func<object, bool> filter)
       {
           Expect(type, filter);
           return _conditionBuilder;
       }
    }
}