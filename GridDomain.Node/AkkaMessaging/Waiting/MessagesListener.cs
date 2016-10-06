using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class MessagesListener
    {
        private readonly ActorSystem _sys;
        private readonly IActorSubscriber _transport;

        public MessagesListener(ActorSystem sys, IActorSubscriber transport)
        {
            _transport = transport;
            _sys = sys;
        }

        internal Task<object> WaitForCommand(CommandPlan plan)
        {
            var inbox = Inbox.Create(_sys);
            var props =
                Props.Create(() => new CommandWaiter(inbox.Receiver,plan.Command, plan.ExpectedMessages));
            var waitActor = _sys.ActorOf(props, "Command_waiter_" + plan.Command.Id);

            foreach (var expectedMessage in plan.ExpectedMessages)
                _transport.Subscribe(expectedMessage.MessageType, waitActor);

            return inbox.ReceiveAsync(plan.Timeout);
        }

        public Task<object> WaitAll(params ExpectedMessage[] expect)
        {
            var inbox = Inbox.Create(_sys);
            var props = Props.Create(() => new AllMessageWaiter(inbox.Receiver, expect));

            var waitActor = _sys.ActorOf(props, "All_msg_waiter_" + Guid.NewGuid());

            foreach (var expectedMessage in expect)
                _transport.Subscribe(expectedMessage.MessageType, waitActor);

            return inbox.ReceiveAsync();
        }

        public Task<object> WaitAny(params ExpectedMessage[] expect)
        {
            var inbox = Inbox.Create(_sys);
            var props = Props.Create(() => new AnyMessageWaiter(inbox.Receiver, expect));

            var waitActor = _sys.ActorOf(props, "All_msg_waiter_" + Guid.NewGuid());

            foreach (var expectedMessage in expect)
                _transport.Subscribe(expectedMessage.MessageType, waitActor);

            return inbox.ReceiveAsync();
        }
    }
}