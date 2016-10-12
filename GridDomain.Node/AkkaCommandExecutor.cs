using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    /// <summary>
    /// Executes commands. Should not be used inside actors
    /// </summary>
    public class AkkaCommandExecutor : ICommandExecutor
    {
        private readonly IActorTransport _transport;
        private readonly MessagesListener _listener;

        public AkkaCommandExecutor(ActorSystem system, IActorTransport transport)
        {
            _transport = transport;
            _listener = new MessagesListener(system, transport);
        }

        public void Execute(params ICommand[] commands)
        {
            foreach (var cmd in commands)
                _transport.Publish(cmd);
        }

        public Task<object> Execute(CommandPlan plan)
        {
            var waitTask = _listener.WaitForCommand(plan);

            Execute(plan.Command);

            return waitTask;
        }

        public Task<T> Execute<T>(CommandPlan<T> plan)
        {
            return Execute((CommandPlan)plan).ContinueWithSafeResultCast(result => (T)result);
        }
    }
}