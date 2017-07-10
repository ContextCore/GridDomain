using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs {


    //public class ParralelProjectionProcessor: 
    public class FireAndForgetMessageProcessor : MessageActorProcessor
    {
        public FireAndForgetMessageProcessor(IActorRef processor) : base(processor)
        {
        }

        public override Task Process(object message, Task workInProgress = null)
        {
            ActorRef.Tell(message);
            return workInProgress;
        }
    }
}