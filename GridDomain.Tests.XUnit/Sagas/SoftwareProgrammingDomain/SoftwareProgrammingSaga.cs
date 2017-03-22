using System;
using Automatonymous;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingSaga : Process<SoftwareProgrammingSagaState>
    {
        public static readonly ISagaDescriptor Descriptor = CreateDescriptor();
        private readonly ILogger Log = Serilog.Log.ForContext<SoftwareProgrammingSaga>();

        public SoftwareProgrammingSaga()
        {
            During(Coding,
                   When(GotTired).Then(context =>
                                       {
                                           var sagaData = context.Instance;
                                           var domainEvent = context.Data;
                                           sagaData.PersonId = domainEvent.SourceId;
                                           Log.Verbose("Hello trace string");
                                           Dispatch(new MakeCoffeCommand(domainEvent.SourceId, sagaData.CoffeeMachineId));
                                       }).TransitionTo(MakingCoffee),
                   When(SleptWell).Then(ctx => ctx.Instance.SofaId = ctx.Data.SofaId).TransitionTo(Coding));

            During(MakingCoffee,
                   When(CoffeNotAvailable).Then(context =>
                                                {
                                                    if (context.Data.CoffeMachineId == Guid.Empty)
                                                        throw new UndefinedCoffeMachineException();

                                                    Dispatch(new GoSleepCommand(context.Data.ForPersonId,
                                                                                context.Instance.SofaId));
                                                }).TransitionTo(Sleeping),
                   When(CoffeReady).TransitionTo(Coding));

            During(Sleeping,
                   When(SleptBad).Then(ctx => ctx.Instance.BadSleepPersonId = ctx.Data.Message.PersonId).TransitionTo(Coding),
                   When(SleptWell).Then(ctx => ctx.Instance.SofaId = ctx.Data.SofaId).TransitionTo(Coding));
        }

        public Event<GotTiredEvent> GotTired { get; private set; }
        public Event<CoffeMadeEvent> CoffeReady { get; private set; }
        public Event<SleptWellEvent> SleptWell { get; private set; }
        public Event<Fault<GoSleepCommand>> SleptBad { get; private set; }
        public Event<CoffeMakeFailedEvent> CoffeNotAvailable { get; private set; }

        public State Coding { get; private set; }
        public State MakingCoffee { get; private set; }
        public State Sleeping { get; private set; }

        private static ISagaDescriptor CreateDescriptor()
        {
            var descriptor = SagaDescriptor.CreateDescriptor<SoftwareProgrammingSaga, SoftwareProgrammingSagaState>();

            descriptor.AddStartMessage<GotTiredEvent>();
            descriptor.AddStartMessage<SleptWellEvent>();

            descriptor.AddCommand<MakeCoffeCommand>();
            descriptor.AddCommand<GoSleepCommand>();

            return descriptor;
        }
    }
}