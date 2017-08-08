using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class Initialize
    {
        public Initialize(IActorRef commandExecutorActor)
        {
            CommandExecutorActor = commandExecutorActor;
        }

        public IActorRef CommandExecutorActor { get; }
    }
}