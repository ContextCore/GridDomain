using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node.Actors
{
    public class GridNodeController : TypedActor
    {
        private readonly IPublisher _messagePublisher;
        private readonly IMessageRouteMap _messageRouting;
        private readonly IActorSubscriber _subscriber;

        public GridNodeController(IPublisher transport,
                                  IActorSubscriber subscriber,
                                  IMessageRouteMap messageRouting)
        {
            _subscriber = subscriber;
            _messageRouting = messageRouting;
            _messagePublisher = transport;
            _monitor = new ActorMonitor(Context);
            _listener = new MessagesListener(Context.System, subscriber);
        }

        public void Handle(Start msg)
        {
            _monitor.IncrementMessagesReceived();
            var system = Context.System;
            var routingActor = system.ActorOf(system.DI().Props(msg.RoutingActorType),msg.RoutingActorType.Name);

            var actorMessagesRouter = new ActorMessagesRouter(routingActor, new DefaultAggregateActorLocator());
            _messageRouting.Register(actorMessagesRouter);

            //TODO: replace with message from router
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(3), Sender, new Started(), Self);
        }

        public void Handle(ICommand cmd)
        {
            _monitor.IncrementMessagesReceived();
            _messagePublisher.Publish(cmd);
        }

        public void Handle(CommandPlan commandWithConfirmation)
        {
           // CreateWaiter(commandWithConfirmation);
            _listener.WaitForCommand(commandWithConfirmation).PipeTo(Sender);
            Handle(commandWithConfirmation.Command);
        }

        //public IActorRef CreateWaiter(CommandPlan commandWithConfirmation)
        //{
        //    var props =
        //        Props.Create(() => new CommandWaiter(Sender, commandWithConfirmation.Command, commandWithConfirmation.ExpectedMessages));
        //    var waitActor = Context.System.ActorOf(props, "MessageWaiter_command_" + commandWithConfirmation.Command.Id);

        //    foreach (var expectedMessage in commandWithConfirmation.ExpectedMessages)
        //        _subscriber.Subscribe(expectedMessage.MessageType, waitActor);

        //    return waitActor;
        //}

        public class Start
        {
            public Type RoutingActorType;
        }

        public class Started
        {
        }

        private readonly ActorMonitor _monitor;
        private readonly MessagesListener _listener;

        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
        }
        protected override void PreRestart(Exception reason, object message)
        {
            _monitor.IncrementActorRestarted();
        }
    }
}