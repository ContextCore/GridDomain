using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    public class MessageWaiterFactory : IMessageWaiterFactory
    {
        public MessageWaiterFactory(ActorSystem system, IActorTransport transport, TimeSpan defaultTimeout)
        {
            System = system;
            DefaultTimeout = defaultTimeout;
            Transport = transport;
        }

        public IActorTransport Transport { get; }
        public TimeSpan DefaultTimeout { get; }
        public ActorSystem System { get; }

        public IMessageWaiter<Task<IWaitResults>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            var conditionBuilder = new MetadataConditionBuilder<Task<IWaitResults>>();
            var waiter = new LocalMessagesWaiter<Task<IWaitResults>>(System, Transport, defaultTimeout ?? DefaultTimeout, conditionBuilder);
            conditionBuilder.CreateResultFunc = () => waiter.Start();
            return waiter;
        }

        public IMessageWaiter<Task<IWaitResults>> NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            var conditionBuilder = new ConditionBuilder<Task<IWaitResults>>();
            var waiter = new LocalMessagesWaiter<Task<IWaitResults>>(System, Transport, defaultTimeout ?? DefaultTimeout, conditionBuilder);
            conditionBuilder.CreateResultFunc = () => waiter.Start();
            return waiter;
        }
    }
}