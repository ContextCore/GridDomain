using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Transport;

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

        public Task Execute(ICommand command, IMessageMetadata metadata = null)
        {
            return _commandExecutorActor.Ask<object>(new MessageMetadataEnvelop(command, metadata ?? CreateEmptyCommandMetadata(command)))
                                        .ContinueWith(t =>
                                                      {
                                                          if(t.Result is CommandExecuted commandCompelted)
                                                              return commandCompelted; 

                                                          if (t.Result is IFault fault)
                                                              throw fault.Exception;

                                                          throw new InvalidMessageException($"expected {typeof(CommandExecuted)} , but received{t.Result.GetType()}");
                                                      });
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
            return new MessageMetadata(cmd.Id.ToString(),
                                       Guid.NewGuid().ToString(),
                                       string.Empty,
                                       new ProcessHistory(new[] {ExecuteMetadataEntry}));
        }
    }
}