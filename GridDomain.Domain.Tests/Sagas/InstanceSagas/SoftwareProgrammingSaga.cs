using System.Runtime.Remoting.Channels;
using Automatonymous;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Logging;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NMoneys;


namespace GridDomain.Tests.Sagas.InstanceSagas
{
    class SoftwareProgrammingSaga: Saga<SoftwareProgrammingSagaData>
    {
        public static readonly ISagaDescriptor Descriptor
            = SagaExtensions.CreateDescriptor<SoftwareProgrammingSaga,
                                              SoftwareProgrammingSagaData,
                                              GotTiredEvent,
                                              SleptWellEvent>();
        
        public SoftwareProgrammingSaga()
        { 
            Event(() => GotTired);
            Event(() => CoffeReady);
            Event(() => SleptWell);
            Event(() => CoffeNotAvailable);
            Event(() => SleptBad);

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
                    Dispatch(new MakeCoffeCommand(domainEvent.SourceId,sagaData.CoffeeMachineId));
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
                When(SleptBad).Then(ctx => ctx.Instance.PersonId = ctx.Data.Message.PersonId)
                              .TransitionTo(MakingCoffee),
                When(SleptWell).Then(ctx => ctx.Instance.SofaId = ctx.Data.SofaId)
                               .TransitionTo(Coding));
        }

        public Event<GotTiredEvent>      GotTired      { get; private set; } 
        public Event<CoffeMadeEvent>      CoffeReady      { get; private set; }
        public Event<SleptWellEvent>     SleptWell     { get; private set; } 
        public Event<Fault<GoSleepCommand>>     SleptBad     { get; private set; } 
        public Event<CoffeMakeFailedEvent> CoffeNotAvailable { get; private set; }

        public State Coding       { get; private set; }
        public State MakingCoffee { get; private set; }
        public State Sleeping  { get; private set; }
    }
}