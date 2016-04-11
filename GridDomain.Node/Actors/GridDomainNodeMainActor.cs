using System;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using CommonDomain.Persistence;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.DomainEventsPublishing;
using GridDomain.Node.MessageRouteConfigs;
using NEventStore;
using NLog;

namespace GridDomain.Node.Actors
{
    public class GridDomainNodeMainActor : TypedActor
    {
        private readonly IStoreEvents _eventStore;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IPublisher _messagePublisher;
        private readonly Func<BusinessBalanceContext> _readContextFactory;
        private readonly IRepository _repo;

        public GridDomainNodeMainActor(IRepository repo,
            IStoreEvents eventStore,
            Func<BusinessBalanceContext> readContextFactory,
            IPublisher transport)
        {
            _eventStore = eventStore;
            _readContextFactory = readContextFactory;
            _repo = repo;
            _messagePublisher = transport;
            _log.Debug($"Актор {GetType().Name} был создан по адресу: {Self.Path}.");
        }

        public void Handle(Start msg)
        {
            _log.Debug($"Актор {GetType().Name} начинает инициализацию");

            CompositionRoot.ConfigurePushingEventsFromStoreToBus(_eventStore,
                                                                 new DomainEventsBusNotifier(_messagePublisher));

            ActorSystem system = Context.System;
            var routingActor = system.ActorOf(system.DI().Props<AkkaRoutingActor>());

            var actorMessagesRouter = new ActorMessagesRouter(routingActor);
            MessageRouting.Init(_repo,
                                _messagePublisher,
                                _readContextFactory,
                                actorMessagesRouter
                                );

            //TODO: replace with message from router
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(3), Sender, new Started(), Self);
        }

        public void Handle(ExecuteCommand message)
        {
            _log.Trace($"Актор {GetType().Name} получил сообщение:\r\n {message.ToPropsString()}");
            _messagePublisher.Publish(message.Command);
        }

        protected override void PostStop()
        {
            _log.Debug($"Актор {GetType().Name} был остановлен");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _log.Debug($"Актор {GetType().Name} будет перезапущен");
            base.PreRestart(reason, message);
        }

        protected override void Unhandled(object message)
        {
            _log.Debug($"Актор {GetType().Name} не смог обработать сообщение:\r\n {message.ToPropsString()}");
            base.Unhandled(message);
        }

        public class ExecuteCommand
        {
            public ExecuteCommand(ICommand command)
            {
                Command = command;
            }

            public ICommand Command { get; }
        }

        public class Start
        {
        }

        public class Started
        {
        }
    }
}