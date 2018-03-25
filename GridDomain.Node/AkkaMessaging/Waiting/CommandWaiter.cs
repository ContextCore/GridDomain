using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Transport;

namespace GridDomain.Node.AkkaMessaging.Waiting
{


    public class CommandWaiter<TCommand> : MessagesWaiter<Task<IWaitResult>>,
                                           ICommandWaiter where TCommand : ICommand
    {
        private readonly CommandConditionBuilder<TCommand> _conditionBuilder;

        public CommandWaiter(TCommand command,
                             IMessageMetadata commandMetadata,
                             ActorSystem system,
                             IActorTransport transport,
                             ICommandExecutor executor,
                             TimeSpan defaultTimeout) : this(system, transport, defaultTimeout, 
                                                             new CommandConditionBuilder<TCommand>(command, commandMetadata, executor)) { }

        public CommandWaiter(ActorSystem system,
                             IActorSubscriber subscriber,
                             TimeSpan defaultTimeout,
                             CommandConditionBuilder<TCommand> conditionBuilder) : base(system, subscriber, defaultTimeout, conditionBuilder.ConditionaBuilder)
        {
            _conditionBuilder = conditionBuilder;
            _conditionBuilder.ConditionaBuilder.CreateResultFunc = Start;
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