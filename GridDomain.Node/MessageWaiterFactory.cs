using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;

using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Transport;

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

        public IMessageWaiter<Task<IWaitResult>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            var conditionBuilder = new MetadataConditionBuilder<Task<IWaitResult>>();
            var waiter = new LocalMessagesWaiter<Task<IWaitResult>>(System, Transport, defaultTimeout ?? DefaultTimeout, conditionBuilder);
            conditionBuilder.CreateResultFunc = waiter.Start;
            return waiter;
        }

        public IMessageWaiter<Task<IWaitResult>> NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            var conditionBuilder = new ConditionBuilder<Task<IWaitResult>>();
            var waiter = new LocalMessagesWaiter<Task<IWaitResult>>(System, Transport, defaultTimeout ?? DefaultTimeout, conditionBuilder);
            conditionBuilder.CreateResultFunc = waiter.Start;
            return waiter;
        }
    }
}