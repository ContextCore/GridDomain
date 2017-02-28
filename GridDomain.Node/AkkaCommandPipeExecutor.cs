using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    public class AkkaCommandPipeExecutor : ICommandExecutor
    {
        private readonly IActorRef _commandExecutorActor;
        private static readonly ProcessEntry ExecuteMetadataEntry = new ProcessEntry(nameof(AkkaCommandPipeExecutor),
                                                                                     "sending command to executor actor",
                                                                                     "command is executing");

        private readonly ActorSystem _system;
        private readonly IActorTransport _transport;
        private readonly TimeSpan _defaultTimeout;

        public AkkaCommandPipeExecutor(ActorSystem system, IActorTransport transport, IActorRef commandExecutorActor, TimeSpan defaultTimeout)
        {
            _defaultTimeout = defaultTimeout;
            _transport = transport;
            _system = system;
            _commandExecutorActor = commandExecutorActor;
        }

        public void Execute<T>(T command, IMessageMetadata metadata = null) where T : ICommand
        {
            _commandExecutorActor.Tell(new MessageMetadataEnvelop<ICommand>(command, metadata));
        }

        public ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            var commandMetadata = metadata ?? new MessageMetadata (cmd.Id,
                                                                  BusinessDateTime.UtcNow,
                                                                  Guid.NewGuid(),
                                                                  Guid.Empty,
                                                                  new ProcessHistory(new[] {ExecuteMetadataEntry}));

            return new CommandWaiter<T>(cmd, commandMetadata, _system, _transport, _commandExecutorActor, _defaultTimeout);
        }
    }
}