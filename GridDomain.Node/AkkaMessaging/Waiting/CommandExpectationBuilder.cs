using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Transport;

namespace GridDomain.Node.AkkaMessaging.Waiting
{


    public class CommandExpectationBuilder<TCommand> : MessagesWaiter,
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
                             CommandEventsFilter<TCommand> eventsFilter) : base(system, subscriber, defaultTimeout, eventsFilter.ConditionFactory)
        {
            _eventsFilter = eventsFilter;
            _eventsFilter.ConditionFactory.CreateResultFunc = Start;
        }


       ICommandEventsFilter<TMsg> ICommandExpectationBuilder.Expect<TMsg>(Predicate<TMsg> filter) 
       {
           Expect(filter);
           return new CommandEventsFilterTypedDecorator<TMsg>(_eventsFilter);
       }

       public ICommandEventsFilter Expect(Type type, Func<object, bool> filter = null)
       {
           ConditionFactory.And(type, filter);
           return _eventsFilter;
       }
    }
}