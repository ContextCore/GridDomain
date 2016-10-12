using System;
using System.Linq.Expressions;
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
            var props = Props.Create(() => new CommandWaiterActor(null,plan.Command, plan.ExpectedMessages));
            var waitActor = _sys.ActorOf(props, "Command_waiter_" + plan.Command.Id);

            foreach (var expectedMessage in plan.ExpectedMessages)
                _transport.Subscribe(expectedMessage.MessageType, waitActor);

            var localWaiter = new LocalMessageWaiter(waitActor, plan.Timeout);
            return localWaiter.ReceiveAll<object>();
        }

        public Task<object> WaitAll(TimeSpan timeout, params ExpectedMessage[] expect)
        {
            var inbox = Inbox.Create(_sys);
            var props = Props.Create(() => new AllMessageWaiterActor(inbox.Receiver, expect));

            var waitActor = _sys.ActorOf(props, "All_msg_waiter_" + Guid.NewGuid());

            foreach (var expectedMessage in expect)
                _transport.Subscribe(expectedMessage.MessageType, waitActor);

            var localWaiter = new LocalMessageWaiter(waitActor, timeout);
            return localWaiter.ReceiveAll<object>();
        }
    }
}