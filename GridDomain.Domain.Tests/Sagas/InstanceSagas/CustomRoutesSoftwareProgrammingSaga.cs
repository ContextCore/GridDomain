using Automatonymous;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Logging;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    class CustomRoutesSoftwareProgrammingSaga : Saga<SoftwareProgrammingSagaData>
    {
        public static readonly ISagaDescriptor Descriptor
            = SagaExtensions.CreateDescriptor<CustomRoutesSoftwareProgrammingSaga,
                SoftwareProgrammingSagaData,
                GotTiredEvent,
                SleptWellEvent>();

        public CustomRoutesSoftwareProgrammingSaga()
        {
            Event(() => GotTired, e => e.PersonId);
            Event(() => CoffeReady, e => e.ForPersonId);
            Event(() => SleptWell, e => e.PersonId);
            Event(() => CoffeNotAvailable, e => e.ForPersonId);

            State(() => Coding);
            State(() => MakingCoffee);
            State(() => Sleeping);

            Command<MakeCoffeCommand>();
            Command<GoSleepCommand>();

            During(Coding,
                When(GotTired).Then(context =>
                {
                    var sagaData = context.Instance;
                    var domainEvent = context.Data;
                    sagaData.PersonId = domainEvent.SourceId;
                    var soloLogger = LogManager.GetLogger();
                    soloLogger.Trace("Hello trace string");
                    Dispatch(new MakeCoffeCommand(domainEvent.SourceId, sagaData.CoffeeMachineId));
                })
                    .TransitionTo(MakingCoffee));

            During(MakingCoffee,
                When(CoffeNotAvailable)
                    .Then(context =>
                        Dispatch(new GoSleepCommand(context.Data.ForPersonId, context.Instance.SofaId)))
                    .TransitionTo(Sleeping),
                When(CoffeReady)
                    .TransitionTo(Coding));

            During(Sleeping,
                When(SleptWell).Then(ctx => ctx.Instance.SofaId = ctx.Data.SofaId)
                    .TransitionTo(Coding));
        }

        public Event<GotTiredEvent> GotTired { get; private set; }
        public Event<CoffeMadeEvent> CoffeReady { get; private set; }
        public Event<SleptWellEvent> SleptWell { get; private set; }
        public Event<CoffeMakeFailedEvent> CoffeNotAvailable { get; private set; }

        public State Coding { get; private set; }
        public State MakingCoffee { get; private set; }
        public State Sleeping { get; private set; }
    }
}