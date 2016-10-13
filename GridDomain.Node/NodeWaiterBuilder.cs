using System;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    public class AkkaMessagesWaiterBuilder : IMessageWaiterProducer
    {
        private readonly ActorSystem _system;
        private readonly IActorSubscriber _subscriber;
        private readonly TimeSpan _timeout;
        private readonly ICommandExecutor _executor;

        public AkkaMessagesWaiterBuilder(ActorSystem system, IActorSubscriber subscriber, TimeSpan timeout, ICommandExecutor executor)
        {
            _executor = executor;
            _timeout = timeout;
            _subscriber = subscriber;
            _system = system;
        }

        public IMessagesWaiterBuilder<IMessageWaiter> Expect()
        {
            return new MessageWaiterConfigurator<LocalMessageWaiter>(
                                    () => LocalMessageWaiter.New(_system, _timeout), _subscriber);
        }

        public IMessagesWaiterBuilder<ICommandWaiter> ExpectCommand()
        {
            return new MessageWaiterConfigurator<CommandMessageWaiter>(
                                            () => CommandMessageWaiter.New(_executor,_system, _timeout), 
                                            _subscriber);

        }
    }
}