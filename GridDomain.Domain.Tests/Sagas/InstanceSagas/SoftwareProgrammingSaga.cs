using System.Runtime.Remoting.Channels;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Logging;
using GridDomain.Tests.Sagas.InstanceSagas.Commands;
using GridDomain.Tests.Sagas.InstanceSagas.Events;
using NMoneys;


namespace GridDomain.Tests.Sagas.InstanceSagas
{
    class SoftwareProgrammingSaga: Saga<SoftwareProgrammingSagaData>
    {
        public SoftwareProgrammingSaga():base(typeof(GotTiredDomainEvent))
        { 
            Event(() => GotTired);
            Event(() => CoffeReady);
            Event(() => SleptWell);
            Event(() => CoffeNotAvailable);

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
                 //   sagaData.Price = new Money(100);
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
                When(SleptWell).TransitionTo(Coding));
        }

        public Event<GotTiredDomainEvent>      GotTired      { get; private set; } 
        public Event<CoffeMadeDomainEvent>      CoffeReady      { get; private set; }
        public Event<SleptWellDomainEvent>     SleptWell     { get; private set; } 
        public Event<CoffeMakeFailedDomainEvent> CoffeNotAvailable { get; private set; }

        public State Coding       { get; private set; }
        public State MakingCoffee { get; private set; }
        public State Sleeping  { get; private set; }
    }
}