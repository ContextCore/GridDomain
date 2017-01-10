using Automatonymous;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Logging;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    class CustomRoutesSoftwareProgrammingSaga : Saga<SoftwareProgrammingSagaData>
    {
        public static readonly ISagaDescriptor Descriptor  = CreateDescriptor();

        private static SagaDescriptor CreateDescriptor()
        {
            var descriptor = SagaDescriptor.CreateDescriptor<CustomRoutesSoftwareProgrammingSaga,
                                                             SoftwareProgrammingSagaData>();

            descriptor.AddStartMessage<GotTiredEvent>();
            descriptor.AddStartMessage<SleptWellEvent>();
            descriptor.AddCommand<MakeCoffeCommand>();
            descriptor.AddCommand<GoSleepCommand>();

            descriptor.MapDomainEvent(s => s.GotTired, e => e.PersonId);
            descriptor.MapDomainEvent(s => s.CoffeReady, e => e.ForPersonId);
            descriptor.MapDomainEvent(s => s.SleptWell, e => e.SofaId);
            descriptor.MapDomainEvent(s => s.CoffeNotAvailable, e => e.CoffeMachineId);
            descriptor.MapDomainEvent(s => s.Custom, e => e.SagaId);

            return descriptor;
        }


        public CustomRoutesSoftwareProgrammingSaga()
        {
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
        public Event<CustomEvent> Custom { get; private set; }
        public Event<CoffeMadeEvent> CoffeReady { get; private set; }
        public Event<SleptWellEvent> SleptWell { get; private set; }
        public Event<CoffeMakeFailedEvent> CoffeNotAvailable { get; private set; }

        public State Coding { get; private set; }
        public State MakingCoffee { get; private set; }
        public State Sleeping { get; private set; }
    }
}