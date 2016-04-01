using System;
using System.Threading;
using Akka.Actor;
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
        private readonly IPublisher _messagePublisher;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IRepository _repo;
        private readonly Func<BusinessBalanceContext> _readContextFactory;
        private readonly IStoreEvents _eventStore;

        public GridDomainNodeMainActor(IRepository repo, 
                                     IStoreEvents eventStore, 
                                     Func<BusinessBalanceContext> readContextFactory,
                                     IPublisher transport)
        {
            _eventStore = eventStore;
            _readContextFactory = readContextFactory;
            _repo = repo;
            _messagePublisher = transport;
            _log.Debug($"Актор {this.GetType().Name} был создан по адресу: {Self.Path}.");
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

        public void Handle(Start msg)
        {
            _log.Debug($"Актор {this.GetType().Name} начинает инициализацию");

            CompositionRoot.ConfigurePushingEventsFromStoreToBus(_eventStore,
                             new DomainEventsBusNotifier(_messagePublisher));

            MessageRouting.Init(_repo,
                                _messagePublisher,
                                _readContextFactory,
                                new ActorMessagesRouter(Context.System, this.Self)
                                );
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Sender.Tell(new Started());
        }

        public void Handle(ExecuteCommand message)
        {
            _log.Trace($"Актор {this.GetType().Name} получил сообщение:\r\n {message.ToPropsString()}");
            _messagePublisher.Publish(message.Command);
        }

        protected override void PostStop()
        {
            _log.Debug($"Актор {this.GetType().Name} был остановлен");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _log.Debug($"Актор {this.GetType().Name} будет перезапущен");
            base.PreRestart(reason, message);
        }

        protected override void Unhandled(object message)
        {
            _log.Debug($"Актор {this.GetType().Name} не смог обработать сообщение:\r\n {message.ToPropsString()}");
            base.Unhandled(message);
        }
    }
}