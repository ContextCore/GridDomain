using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Event;
using GridDomain.Aggregates;
using GridDomain.Node.Akka.Actors.Aggregates;

namespace GridDomain.Node
{
    public class ActorCommandExecutor : ICommandHandler<ICommand>
    {
        private readonly IActorRef _commandExecutorActor;
        private readonly TimeSpan _timeout;
        private readonly ILoggingAdapter _log;

        public ActorCommandExecutor(IActorRef commandExecutorActor, ILoggingAdapter log, TimeSpan? timeout = null)
        {
            _log = log;
            _commandExecutorActor = commandExecutorActor;
            _timeout = timeout ?? TimeSpan.FromSeconds(5);
        }

        public async Task<object> Execute(ICommand command)
        {
            _log.Debug("Preparing command {@command}",command);
            var envelopedCommand = ShardedAggregateCommand.New(command, true, MessageMetadata.New(command.Id, Guid.NewGuid().ToString()));

            var result = await _commandExecutorActor.Ask<AggregateActor.CommandExecuted>(envelopedCommand,_timeout);
            if (result is AggregateActor.CommandFailed fail)
            {
                if (fail.Reason is AggregateActor.CommandExecutionException e && e.InnerException != null) throw e.InnerException;
                throw fail.Reason;
            }

            return result.Value;
        }
    }
}