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

        public IMessageWaiter<Task<IWaitResult>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            var conditionBuilder = new LocalMetadataConditionFactory<Task<IWaitResult>>();
            var conditionFactory = new ConditionFactory<Task<IWaitResult>>(conditionBuilder);
            var waiter = new MessagesWaiter<Task<IWaitResult>>(System, Transport, defaultTimeout ?? DefaultTimeout, conditionFactory);
            conditionFactory.CreateResultFunc = waiter.Start;
            return waiter;
        }

        public IMessageWaiter<Task<IWaitResult>> NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            var conditionBuilder = new ConditionFactory<Task<IWaitResult>>();
            var waiter = new MessagesWaiter<Task<IWaitResult>>(System, Transport, defaultTimeout ?? DefaultTimeout, conditionBuilder);
            conditionBuilder.CreateResultFunc = waiter.Start;
            return waiter;
        }
    }
}