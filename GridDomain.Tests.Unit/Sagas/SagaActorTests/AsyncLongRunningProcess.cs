using System.Threading.Tasks;
using Automatonymous;
using GridDomain.Processes;
using GridDomain.Processes.DomainBind;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.Sagas.SagaActorTests
{
    public class AsyncLongRunningProcess : Process<TestState>
    {
        public AsyncLongRunningProcess()
        {
            InstanceState(s => s.CurrentStateName);

            During(Initial,
                   When(Start).ThenAsync(async (state, msg) =>{
                                             state.ProcessingId = msg.SourceId;
                                             await Task.Delay(100);
                                         }).TransitionTo(Initial),
                   When(Progress).Then((state,msg) =>
                                       {
                                           state.ProcessingId = msg.SourceId;
                                       }).TransitionTo(Final));
        }

        public static IProcessManagerDescriptor Descriptor
        {
            get
            {
                var descriptor = ProcessManagerDescriptor.CreateDescriptor<AsyncLongRunningProcess, TestState>();
                descriptor.AddStartMessage<BalloonCreated>();
                descriptor.AddAcceptedMessage<BalloonTitleChanged>();
                return descriptor;
            }
        }

        public Event<BalloonCreated> Start { get; private set; }
        public Event<BalloonTitleChanged> Progress { get; private set; }
    }
}