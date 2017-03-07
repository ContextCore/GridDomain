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
        private static readonly ProcessEntry ExecuteMetadataEntry = new ProcessEntry(nameof(AkkaCommandPipeExecutor),
                                                                                     "sending command to executor actor",
                                                                                     "command is executing");

        private readonly IActorRef _commandExecutorActor;
        private readonly TimeSpan _defaultTimeout;

        private readonly ActorSystem _system;
        private readonly IActorTransport _transport;

        public AkkaCommandPipeExecutor(ActorSystem system,
                                       IActorTransport transport,
                                       IActorRef commandExecutorActor,
                                       TimeSpan defaultTimeout)
        {
            _defaultTimeout = defaultTimeout;
            _transport = transport;
            _system = system;
            _commandExecutorActor = commandExecutorActor;
        }

        public void Execute<T>(T command, IMessageMetadata metadata = null) where T : ICommand
        {
            _commandExecutorActor.Tell(new MessageMetadataEnvelop<ICommand>(command, metadata ?? CreateEmptyCommandMetadata(command)));
        }

        public ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            return new CommandWaiter<T>(cmd,
                                        metadata ?? CreateEmptyCommandMetadata(cmd),
                                        _system,
                                        _transport,
                                        _commandExecutorActor,
                                        _defaultTimeout);
        }

        private static MessageMetadata CreateEmptyCommandMetadata<T>(T cmd) where T : ICommand
        {
            return new MessageMetadata(cmd.Id,
                                       BusinessDateTime.UtcNow,
                                       Guid.NewGuid(),
                                       Guid.Empty,
                                       new ProcessHistory(new[] {ExecuteMetadataEntry}));
        }
    }
}