using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;

using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Transport;

namespace GridDomain.Node
{
    public class LocalMessageWaiterFactory : IMessageWaiterFactory
    {
        public LocalMessageWaiterFactory(ActorSystem system, IActorTransport transport, TimeSpan defaultTimeout)
        {
            System = system;
            DefaultTimeout = defaultTimeout;
            Transport = transport;
        }

        public IActorTransport Transport { get; }
        public TimeSpan DefaultTimeout { get; }
        public ActorSystem System { get; }

        public IMessageWaiter NewWaiter(TimeSpan? defaultTimeout = null)
        {
            var conditionBuilder = new LocalMetadataEnvelopConditionBuilder();
            var conditionFactory = new MessageConditionFactory<Task<IWaitResult>>(conditionBuilder);
            var waiter = new MessagesWaiter(System, Transport, defaultTimeout ?? DefaultTimeout, conditionFactory);
            return waiter;
        }

        public IMessageWaiter NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            
            var conditionBuilder = new MessageConditionFactory<Task<IWaitResult>>();
            var waiter = new MessagesWaiter(System, Transport, defaultTimeout ?? DefaultTimeout, conditionBuilder);
            return waiter;
        }
    }
}