using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Transport;

namespace GridDomain.Node.Cluster {
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

        public IMessageWaiter<Task<IWaitResult>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            var conditionBuilder = new MetadataConditionBuilder<Task<IWaitResult>>();
            var waiter = new MessagesWaiter<Task<IWaitResult>>(System, Transport, defaultTimeout ?? DefaultTimeout, conditionBuilder);
            conditionBuilder.CreateResultFunc = waiter.Start;
            return waiter;
        }

        public IMessageWaiter<Task<IWaitResult>> NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            throw new NotSupportedException("Cluster cannot wait explicit messages due to topic-based pub-sub");
        }
    }
}