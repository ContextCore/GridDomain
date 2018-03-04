using System;
using System.Threading.Tasks;
using Akka.Actor;
using DotNetty.Handlers;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Transport;
using TimeoutException = System.TimeoutException;

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

        public async Task Execute(ICommand command, IMessageMetadata metadata = null, CommandConfirmationMode confirmationMode = CommandConfirmationMode.Projected)
        {
            var envelopedCommand = new MessageMetadataEnvelop(command, metadata ?? CreateEmptyCommandMetadata(command));

            if (confirmationMode == CommandConfirmationMode.None)
            {
                _commandExecutorActor.Tell(envelopedCommand);
                return;
            }

            var inbox = Inbox.Create(_system);
            _commandExecutorActor.Tell(envelopedCommand, inbox.Receiver);

            var msg = await inbox.ReceiveAsync(_defaultTimeout);

            if(CheckMessage(confirmationMode, msg)) return;

            msg = await inbox.ReceiveAsync(_defaultTimeout);

            if (CheckMessage(confirmationMode, msg)) return;
            
            throw new TimeoutException("Command execution took to long");
            
        }

        private static bool CheckMessage(CommandConfirmationMode confirmationMode, object msg)
        {
            switch (msg)
            {
                case AggregateActor.CommandProjected _:
                    if (confirmationMode == CommandConfirmationMode.Projected) return true;
                    break;
                case AggregateActor.CommandExecuted _:
                    if (confirmationMode == CommandConfirmationMode.Executed) return true;
                    break;
                case IFault fault:
                    throw fault.Exception;
                case Status.Failure failure:
                    throw failure.Cause;
                default:
                    throw new InvalidMessageException($"unexpected message received {msg.GetType()}");
            }

            return false;
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
                Guid.NewGuid()
                    .ToString(),
                string.Empty,
                new ProcessHistory(new[] {ExecuteMetadataEntry}));
        }
    }
}