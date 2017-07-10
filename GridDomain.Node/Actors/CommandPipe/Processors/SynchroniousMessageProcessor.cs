using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.Hadlers;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs {


    public class SyncProjectionProcessor : SynchroniousMessageProcessor<HandlerExecuted>
    {
        public SyncProjectionProcessor(IActorRef processor) : base(processor) { }
    }
    public class SyncSagaProcessor: SynchroniousMessageProcessor<ISagaTransitCompleted>
    {
        public SyncSagaProcessor(IActorRef processor) : base(processor) { }
    }

    public class SynchroniousMessageProcessor<T>: MessageActorProcessor
    {
        public SynchroniousMessageProcessor(IActorRef processor):base(processor)
        {
        }

        public override Task Process(object message, Task workInProgress = null)
        {
            if(workInProgress == null || workInProgress.IsCompleted)
                return AskActor<T>(message);

            return workInProgress.ContinueWith(t => AskActor<T>(message));
        }
    }
}