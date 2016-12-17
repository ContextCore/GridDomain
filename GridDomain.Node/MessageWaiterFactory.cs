using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    public class MessageWaiterFactory : IMessageWaiterFactory, ICommandWaiterFactory
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
        public ICommandWaiter<T> PrepareCommand<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            var commandMetadata = metadata ?? new MessageMetadata(cmd.Id,
                                                                  BusinessDateTime.UtcNow,
                                                                  Guid.NewGuid(),
                                                                  Guid.Empty,
                                                                  new ProcessHistory(new[] {
                                                                    new ProcessEntry(nameof(AkkaCommandExecutor),
                                                                                  "publishing command to transport",
                                                                                  "command is executing")}));

            return new CommandWaiter<T>(cmd,metadata,System,Transport, DefaultTimeout);

        }
    }
}