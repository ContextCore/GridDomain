using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka;
using Akka.Actor;
using Akka.Dispatch;
using Akka.DI.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Routing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using Quartz.Collection;

namespace GridDomain.Node.Actors
{
    public class GridDomainNodeMainActor : TypedActor
    {
        private readonly ISoloLogger _log = LogManager.GetLogger();
        private readonly IPublisher _messagePublisher;
        private readonly IMessageRouteMap _messageRouting;
        private readonly IActorSubscriber _subscriber;

        public GridDomainNodeMainActor(IPublisher transport,
                                       IActorSubscriber subscriber,
                                       IMessageRouteMap messageRouting,
                                       IServiceLocator locator)
        {
            _subscriber = subscriber;
            _messageRouting = messageRouting;
            _messagePublisher = transport;
            _log.Debug($"Актор {GetType().Name} был создан по адресу: {Self.Path}.");
        }

        public void Handle(Start msg)
        {
            _log.Debug($"Актор {GetType().Name} начинает инициализацию");

            var system = Context.System;
            var routingActor = system.ActorOf(system.DI().Props(msg.RoutingActorType),msg.RoutingActorType.Name);

            var actorMessagesRouter = new ActorMessagesRouter(routingActor, new DefaultAggregateActorLocator());
            _messageRouting.Register(actorMessagesRouter);

            //TODO: replace with message from router
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(3), Sender, new Started(), Self);
        }

        public void Handle(ExecuteCommand message)
        {
            _log.Trace($"Актор {GetType().Name} получил сообщение:\r\n {message.ToPropsString()}");
            _messagePublisher.Publish(message.Command);
        }

        IDictionary<Guid, CommandWaiter> executingCommandsByEvent = new Dictionary<Guid, CommandWaiter>();
        IDictionary<Guid, CommandWaiter> executingCommands = new Dictionary<Guid, CommandWaiter>();


        class CommandWaiter
        {
            public ICommand Command;
            public IActorRef Waiter;
        }
        public void Handle(ExecuteConfirmedCommand message)
        {
            var msgToWait = message.Command.ExpectedMessages.Select(c => new MessageToWait(c, 1)).ToArray();
            var executor = Context.System.ActorOf(Props.Create(() => new MessageWaiter(Self, msgToWait)),"MessageWaiter_command_"+message.Command.Id);
            
            //TODO : filter messages in waiter!!!!
            var executingCommand = new CommandWaiter()
            {
                Command = message.Command,
                Waiter = Sender
            };

            executingCommandsByEvent[message.Command.EventId] = executingCommand;
            executingCommands[message.Command.Id] = executingCommand;


            foreach (var messageToWait in msgToWait)
            {
                _subscriber.Subscribe(messageToWait.MessageType, executor);
            }
            //TODO: replace with ack from subscriber
            Thread.Sleep(500); //to finish subscribe
            _messagePublisher.Publish(message.Command);
        }

        //Finished execution of awaitable command
        public void Handle(ExpectedMessagesRecieved message)
        {
            var resultMessage = message.Message;
            resultMessage.Match()
                         .With<ICommandFault>(f =>
                         {
                             CommandWaiter commandWaiter;
                             if (executingCommands.TryGetValue(f.Command.Id, out commandWaiter))
                                 commandWaiter.Waiter.Tell(f);
                         })
                         .With<DomainEvent>(e => {
                             CommandWaiter commandWaiter;
                             if (executingCommandsByEvent.TryGetValue(e.SourceId, out commandWaiter))
                                 commandWaiter.Waiter.Tell(new CommandExecutionFinished(commandWaiter.Command, e));
                         });
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
            public Type RoutingActorType;
        }

        public class Started
        {
        }

        public class ExecuteConfirmedCommand
        {
            public CommandWithKnownResult Command { get; }
            public ExecuteConfirmedCommand(CommandWithKnownResult command)
            {
                Command = command;
            }
        }
    }

    public class CommandExecutionFinished
    {
        public DomainEvent DomainEvent;
        public ICommand Command { get; set; }

        public CommandExecutionFinished(ICommand command, DomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
            Command = command;
        }
    }
}