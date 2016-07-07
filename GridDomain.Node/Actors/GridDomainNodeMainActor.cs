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
            _log.Debug($"Actor {GetType().Name} was created on: {Self.Path}.");
        }

        public void Handle(Start msg)
        {
            _log.Debug($"Actor {GetType().Name} is initializing");

            var system = Context.System;
            var routingActor = system.ActorOf(system.DI().Props(msg.RoutingActorType),msg.RoutingActorType.Name);

            _log.Debug($"Actor {GetType().Name} is initializing routes");
            var actorMessagesRouter = new ActorMessagesRouter(routingActor, new DefaultAggregateActorLocator());
            _messageRouting.Register(actorMessagesRouter);

            //TODO: replace with message from router
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(3), Sender, new Started(), Self);
            _log.Debug($"Actor {GetType().Name} finished routes initialization");
        }

        public void Handle(ICommand cmd)
        {
            _log.Trace($"Актор {GetType().Name} получил сообщение:\r\n {cmd.ToPropsString()}");
            _messagePublisher.Publish(cmd);
        }


        public void Handle(CommandAndConfirmation commandWithConfirmation)
        {
            var waitAggregate = Context.System.ActorOf(Props.Create(() => new CommandWaiter(Sender, commandWithConfirmation.Command,commandWithConfirmation.ExpectedMessages)),"MessageWaiter_command_"+commandWithConfirmation.Command.Id);

            foreach(var expectedMessage in commandWithConfirmation.ExpectedMessages)
                    _subscriber.Subscribe(expectedMessage.MessageType, waitAggregate);

            //TODO: replace with ack from subscriber
            Thread.Sleep(500); //to finish subscribe
            _messagePublisher.Publish(commandWithConfirmation.Command);
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

        public class Start
        {
            public Type RoutingActorType;
        }

        public class Started
        {
        }
    }

    public class CommandExecutionFinished
    {
        public object ResultMessage { get; }
        public ICommand CommandId { get; }

        public CommandExecutionFinished(ICommand commandId, object resultMessage)
        {
            ResultMessage = resultMessage;
            CommandId = commandId;
        }
    }
}