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
        public MessageWaiterFactory(ICommandExecutor commandExecutor, ActorSystem system, TimeSpan defaultTimeout, IActorTransport transport)
        {
            CommandExecutor = commandExecutor;
            System = system;
            DefaultTimeout = defaultTimeout;
            Transport = transport;
        }

        public IActorTransport Transport { get; }
        public TimeSpan DefaultTimeout { get; }
        public ICommandExecutor CommandExecutor { get; }
        public ActorSystem System { get; }
        
        public IMessageWaiter<Task<IWaitResults>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return new AkkaMessageLocalWaiter(System, Transport, defaultTimeout ?? DefaultTimeout);

        }

        public IMessageWaiter<IExpectedCommandExecutor> NewCommandWaiter(TimeSpan? defaultTimeout = null, bool failAnyFault = true)
        {
            return new AkkaCommandLocalWaiter(CommandExecutor, System, Transport, defaultTimeout ?? DefaultTimeout, failAnyFault);

        }
    }
}