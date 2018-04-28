using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Transport;

namespace GridDomain.Node.Cluster.MessageWaiting {
    public class ClusterMessageWaiterFactory : IMessageWaiterFactory
    {
        public ClusterMessageWaiterFactory(ActorSystem system, IActorTransport transport, TimeSpan defaultTimeout)
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
            var conditionBuilder = new MetadataConditionFactory();
            var conditionFactory = new ConditionFactory<Task<IWaitResult>>(conditionBuilder);
            var waiter = new MessagesWaiter(System, Transport, defaultTimeout ?? DefaultTimeout, conditionFactory);
            return waiter;
        }

        public IMessageWaiter NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            throw new NotSupportedException("Cluster cannot wait explicit messages due to topic-based pub-sub");
        }
    }
}