using System.Runtime.Remoting.Channels;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas.Commands;
using GridDomain.Tests.Sagas.InstanceSagas.Events;


namespace GridDomain.Tests.Sagas.InstanceSagas
{
    class SoftwareProgrammingSaga: Saga<SoftwareProgrammingSagaData>
    {
        public SoftwareProgrammingSaga():base(typeof(GotTiredDomainEvent))
        { 
            Event(() => GotTired);
            Event(() => FeltGood);
            Event(() => SleptWell);
            Event(() => FeltMoreTired);

            State(() => Coding);
            State(() => DrinkingCoffee);
            State(() => Sleeping);

            Command<GoForCoffeCommand>();
            Command<GoSleepCommand>();

            During(Coding,
                When(GotTired).Then(context =>
                {
                    context.Instance.SubscriptionId = context.Data.SourceId;
                    Dispatch(new GoForCoffeCommand(context.Data));
                })
                .TransitionTo(DrinkingCoffee));

            During(DrinkingCoffee, 
                When(FeltMoreTired)
                    .Then(context => Dispatch(new GoSleepCommand(context.Data)))
                    .TransitionTo(Sleeping),
                When(FeltGood)
                    .TransitionTo(Coding));

             During(Sleeping,
                When(SleptWell).TransitionTo(Coding));
        }

        public Event<GotTiredDomainEvent>      GotTired      { get; private set; } 
        public Event<FeltGoodDomainEvent>      FeltGood      { get; private set; }
        public Event<SleptWellDomainEvent>     SleptWell     { get; private set; } 
        public Event<FeltMoreTiredDomainEvent> FeltMoreTired { get; private set; }

        public State Coding       { get; private set; }
        public State DrinkingCoffee { get; private set; }
        public State Sleeping  { get; private set; }
    }
}