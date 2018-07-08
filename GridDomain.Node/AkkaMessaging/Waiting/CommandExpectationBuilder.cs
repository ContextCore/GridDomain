using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Transport;

namespace GridDomain.Node.AkkaMessaging.Waiting
{


    public class CommandExpectationBuilder<TCommand> : MessagesWaiter<ICommandEventsFilter>,
                                                       ICommandExpectationBuilder where TCommand : ICommand
    {
        private readonly CommandEventsFilter<TCommand> _eventsFilter;

        public CommandExpectationBuilder(TCommand command,
                             IMessageMetadata commandMetadata,
                             ActorSystem system,
                             IActorTransport transport,
                             ICommandExecutor executor,
                             TimeSpan defaultTimeout) : this(system, transport, defaultTimeout, 
                                                             new CommandEventsFilter<TCommand>(command, commandMetadata, executor)) { }

        public CommandExpectationBuilder(ActorSystem system,
                             IActorSubscriber subscriber,
                             TimeSpan defaultTimeout,
                             CommandEventsFilter<TCommand> eventsFilter) : base(system, subscriber, defaultTimeout)
        {
            _eventsFilter = eventsFilter;
            _eventsFilter.MessageConditionFactory.CreateResultFunc = Start;
        }

        public override IMessagesExpectation CreateMessagesExpectation()
        {
            return _eventsFilter.MessageConditionFactory.Builder.Build();
        }

        ICommandEventsFilter<TMsg> ICommandExpectationBuilder.Expect<TMsg>(Predicate<TMsg> filter) 
       {
           Expect(filter);
           return new CommandEventsFilterTypedDecorator<TMsg>(_eventsFilter);
       }

        public override ICommandEventsFilter Expect(Type type, Func<object, bool> filter)
        {
             _eventsFilter.MessageConditionFactory.And(type, filter);
            return _eventsFilter;
        }

        public override ICommandEventsFilter Expect<TMsg>(Predicate<TMsg> filter = null)
        {
            _eventsFilter.MessageConditionFactory.And(filter);
            return _eventsFilter;
        }
    }
}