using System.Threading.Tasks;
using Automatonymous;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.Sagas.SagaActorTests
{
    public class AsyncLongRunningSaga : Process<TestState>
    {
        public AsyncLongRunningSaga()
        {
            InstanceState(s => s.CurrentStateName);

            During(Initial,
                   When(Start).ThenAsync(async (state, msg) =>{
                                             state.ProcessingId = msg.SourceId;
                                             await Task.Delay(100);
                                         }).TransitionTo(Initial),
                   When(Progress).Then((state,msg) =>
                                       {
                                           state.ProcessingId = msg.Id;
                                       }).TransitionTo(Final));
        }

        public static ISagaDescriptor Descriptor
        {
            get
            {
                var descriptor = SagaDescriptor.CreateDescriptor<AsyncLongRunningSaga, TestState>();
                descriptor.AddStartMessage<SampleAggregateCreatedEvent>();
                descriptor.AddAcceptedMessage<SampleAggregateChangedEvent>();
                return descriptor;
            }
        }

        public Event<SampleAggregateCreatedEvent> Start { get; private set; }
        public Event<SampleAggregateChangedEvent> Progress { get; private set; }
    }
}