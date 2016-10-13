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
            var localWaiter = CommandMessageWaiter.New(_sys, plan.Timeout);
            foreach (var expectedMessage in plan.ExpectedMessages)
                _transport.Subscribe(expectedMessage.MessageType, localWaiter.Receiver);

            return localWaiter.ReceiveAll<object>();
        }
    }
}